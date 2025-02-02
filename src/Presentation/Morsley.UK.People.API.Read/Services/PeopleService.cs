﻿namespace Morsley.UK.People.API.Read.Services;

internal class PeopleService
{
    internal async static Task<Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>> TryGetPeople(
        GetPeopleRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        var name = $"PeopleService->{nameof(TryGetPeople)}";
        using var activity = source.StartActivity(name, ActivityKind.Server);

        var getPeopleQuery = mapper.Map<GetPeopleQuery>(request);
        var pageOfPeople = await mediator.Send(getPeopleQuery);
        var pageOfPersonResponses = mapper.Map<Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>>(pageOfPeople);
        
        return pageOfPersonResponses;
    }
}