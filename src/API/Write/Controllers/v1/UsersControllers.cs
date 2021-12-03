﻿namespace Users.API.Write.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UsersController : API.Shared.Controllers.v1.BaseController
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(
        ILogger<UsersController> logger,
        IMediator mediator,
        IMapper mapper)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
    }

    /***********************************************************************************************************************
     * POST --> ADD                                                                                                     *
     ***********************************************************************************************************************/                                                                                                                     

    /// <summary>
    /// Add a user
    /// </summary>
    /// <param name="addUserRequest">An AddUserRequest object which contains all the necessary data to create a user</param>
    /// <returns>A URI to the newly added user in the header (location)</returns>
    /// <response code="201">Success - Added - The user was successfully Added</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response> 
    [HttpPost]
    [MapToApiVersion("1.0")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUser([FromBody] Users.API.Models.Request.v1.AddUserRequest addUserRequest)
    {
        if (addUserRequest == null) return BadRequest();

        var addUserCommand = _mapper.Map<Users.Application.Commands.AddUserCommand>(addUserRequest);

        var user = await _mediator.Send(addUserCommand);

        var userResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(user);

        return Created($"http://localhost/api/v1/users/{userResponse.Id}", userResponse);
    }

    /***********************************************************************************************************************
     * DELETE                                                                                                              *
     ***********************************************************************************************************************/

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>Nothing</returns>
    /// <response code="204">Success - No Content - User was successfully deleted</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - No user matched the given identifier</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        var deleteUserRequest = new Users.API.Models.Request.v1.DeleteUserRequest(userId);

        var deleteUserCommand = _mapper.Map<Users.Application.Commands.DeleteUserCommand>(deleteUserRequest);

        await _mediator.Send(deleteUserCommand);

        return NoContent();
    }

    /***********************************************************************************************************************
     * PUT -- UPSERT                                                                                                       *
     ***********************************************************************************************************************/

    /// <summary>
    /// Upsert a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="upsertUserRequest">
    /// An UpsertUserRequest object which contains all the
    /// data required to either update or create a user.
    /// </param>
    /// <returns>The upserted user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="201">Success - Created - The user was successfully created</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

    public async Task<IActionResult> UpsertUser(
        [FromRoute] Guid userId,
        [FromBody] Users.API.Models.Request.v1.UpsertUserRequest upsertUserRequest)
    {
        if (upsertUserRequest == null) return BadRequest();

        if (await DoesUserExist(userId))
        {
            var updateUserCommand = _mapper.Map<Application.Commands.UpdateUserCommand>(upsertUserRequest);
            updateUserCommand.Id = userId;

            var updatedUser = await _mediator.Send(updateUserCommand);

            return Ok();
        }

        var createUserRequest = new Users.API.Models.Request.v1.AddUserRequest();
        return RedirectToAction(nameof(AddUser), createUserRequest);
    }

    /***********************************************************************************************************************
     * PATCH --> UPSERT                                                                                                    *
     ***********************************************************************************************************************/

    /// <summary>
    /// Fully or partially update a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="patchDocument">
    /// A JSON Patch Document detailing the full or partial updates to the user
    /// </param>
    /// <returns>The updated user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - No user matched the given identifier</response>
    /// <response code="422">Error - Unprocessable Entity - Unable to process the contained instructions.</response>
    /// <remarks>
    /// Sample request (this request updates the user's first name): \
    /// PATCH /users/id \
    /// [ \
    ///     { \
    ///         "op": "replace", \
    ///         "path": "/firstname", \
    ///         "value": "Bob" \
    ///     } \
    /// ] \
    /// </remarks>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpsertUser(
        [FromRoute] Guid userId,
        [FromBody] JsonPatchDocument<Users.API.Models.Request.v1.PartiallyUpsertUserRequest> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        var partiallyUpsertUserRequest = new Users.API.Models.Request.v1.PartiallyUpsertUserRequest(userId);
        patchDocument.ApplyTo(partiallyUpsertUserRequest, ModelState);
        if (!TryValidateModel(partiallyUpsertUserRequest))
        {
            return ValidationProblem(ModelState);
        }
            
        if (await DoesUserExist(userId))
        {
            var updatedUser = await PartiallyUpdateUser(userId, partiallyUpsertUserRequest);
            return Ok(updatedUser);
        }
            
        var createUserRequest = new Users.API.Models.Request.v1.AddUserRequest();
        return RedirectToAction(nameof(AddUser), createUserRequest);
            
        throw new NotImplementedException();
    }

    /// <summary>
    /// List the HTTP methods allowed
    /// </summary>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetOptions()
    {
        Response.Headers.Add("Allow", "DELETE,OPTIONS,PATCH,POST,PUT");
        return Ok();
    }

    private async Task<bool> DoesUserExist(Guid userId)
    {
        var userExistsQuery = new Application.Queries.UserExistsQuery(userId);

        var doesUserExist = await _mediator.Send(userExistsQuery);
        
        return doesUserExist;
    }
        
    //private async Task<Users.API.Models.Response.v1.UserResponse> UpdateUser(
    //    Guid userId,
    //    Users.API.Models.Request.v1.UpsertUserRequest request)
    //{
    //    if (request == null) throw new ArgumentNullException(nameof(request));

    //    // Update User...
    //    await Task.CompletedTask;
            
    //    throw new NotImplementedException();
    //}

    private async Task<Users.API.Models.Response.v1.UserResponse> PartiallyUpdateUser(
        Guid userId,
        Users.API.Models.Request.v1.PartiallyUpsertUserRequest partiallyUpsertUserRequest)
    {
        if (partiallyUpsertUserRequest == null) throw new ArgumentNullException(nameof(partiallyUpsertUserRequest));
            
        // var partiallyUpdateUserCommand = _mapper.Map<PartiallyUpdateUserCommand>(partiallyUpsertUserRequest);
        //
        // var updatedUser = await _mediator.Send(partiallyUpdateUserCommand);
        //
        // var updatedUserResponse = _mapper.Map<UserResponse>(updatedUser);
        //
        // return updatedUserResponse;

        throw new NotImplementedException();
    }
}
