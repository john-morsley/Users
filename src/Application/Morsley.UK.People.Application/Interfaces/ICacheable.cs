﻿namespace Morsley.UK.People.Application.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }

    bool DoNotCache { get; set; }

    TimeSpan TimeToLive { get; set; }
}
