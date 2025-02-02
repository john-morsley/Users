﻿namespace Morsley.UK.People.API.Contracts.Shared;

public class PropertyMapping<TSource, TDestination> : IPropertyMapping
{
    public Dictionary<string, PropertyMappingValue> Mappings { get; set; }

    public PropertyMapping(Dictionary<string, PropertyMappingValue> mappings)
    {
        Mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
    }
}