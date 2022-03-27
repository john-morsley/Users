﻿namespace Morsley.UK.People.Messaging.Configuration;

internal class RabbitMQSettings
{
    public string? Host { get; set; }

    public string? Port { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? QueueName { get; set; }
}