﻿namespace Morsley.UK.People.Application.Interfaces;

public interface IGetOptions
{
    uint PageSize { get; }

    uint PageNumber { get; }

    string? Search { get; }

    IEnumerable<IFilter> Filters { get; }

    IEnumerable<IOrdering> Orderings { get; }
}