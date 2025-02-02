﻿namespace Morsley.UK.People.API.Write.Endpoints;

/// <summary>
/// 
/// </summary>
public static class DeletePersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapDeletePersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new[] { "DELETE" }, async (
                    [FromRoute] Guid id,
                    [FromServices] IValidator<DeletePersonRequest> validator,
                    [FromServices] IMediator mediator,
                    [FromServices] ILogger logger,
                    [FromServices] ActivitySource source)
                    =>
                    await DeletePerson(new DeletePersonRequest { Id = id }, validator, mediator, logger, source))
                   //.Accepts("application/json")
                   .Produces(StatusCodes.Status204NoContent)
                   .Produces(StatusCodes.Status404NotFound)
                   .Produces(StatusCodes.Status422UnprocessableEntity)
                   .Produces(StatusCodes.Status500InternalServerError)
                   .WithName("DeletePerson").WithDisplayName("Delete Person"); //.WithGroupName("Person");
    }

    [HttpDelete]
    private async static Task<IResult> DeletePerson(
        DeletePersonRequest request,
        IValidator<DeletePersonRequest> validator,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        if (request.Id == Guid.Empty) return Results.BadRequest();

        if (!await DoesPersonExist(request.Id, mediator, logger, source)) return Results.NotFound();

        await TryDeletePerson(request.Id, mediator, logger, source);

        return Results.NoContent();
    }

    private async static Task<bool> DoesPersonExist(
        Guid personId,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            var personExistsQuery = new PersonExistsQuery(personId);
            var doesPersonExist = await mediator.Send(personExistsQuery);
            return doesPersonExist;
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to find a person!");
            return false;
        }
    }

    private async static Task TryDeletePerson(
        Guid personId,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            await PersonHelper.DeletePerson(personId, mediator, logger, source);
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to delete a person!");
        }
    }
}
