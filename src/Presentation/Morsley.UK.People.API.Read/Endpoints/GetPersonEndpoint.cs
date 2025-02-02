﻿namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// The enpoint to use to get a Person.
/// </summary>
public static class GetPersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapGetPersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new [] { "GET" }, async (
                    [FromRoute] Guid id, 
                    [FromQuery] string? fields,
                    IValidator<GetPersonRequest> validator,
                    [FromServices] IMapper mapper,
                    [FromServices] IMediator mediator,
                    [FromServices] ILogger logger,
                    [FromServices] ActivitySource source)
                    =>
                    await GetPerson(new GetPersonRequest { Id = id, Fields = fields }, validator, mapper, mediator, logger, source))
                   //.Accepts<GetPersonRequest>("application/json")
                   .Produces<PersonResponse>(StatusCodes.Status200OK, "application/json")
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status204NoContent)
                   .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")
                   .WithName("GetPerson");
    }

    private async static Task<IResult> GetPerson(
        GetPersonRequest request,
        IValidator<GetPersonRequest> validator,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        logger.Debug("GetPerson");

        var name = $"GetPeopleEndpoint->{nameof(GetPerson)}";
        using var activity = source.StartActivity(name, ActivityKind.Server);

        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        if (request.Id == Guid.Empty) return Results.BadRequest();

        var personResponse = await PersonService.TryGetPerson(request, mapper, mediator, logger, source);

        if (personResponse == null) return Results.NoContent();

        var shapedPerson = personResponse.ShapeData(request.Fields);

        var shapedPersonWithLinks = LinksHelper.AddLinks(shapedPerson!, personResponse.Id);

        return Results.Ok(shapedPersonWithLinks);
    }
}