﻿global using AutoMapper;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Morsley.UK.People.API.Common.Helpers;
global using Morsley.UK.People.API.Common.IoC;
global using Morsley.UK.People.API.Common.Swagger;
global using Morsley.UK.People.API.Contracts.Extensions;
global using Morsley.UK.People.API.Contracts.IoC;
global using Morsley.UK.People.API.Contracts.Requests;
global using Morsley.UK.People.API.Contracts.Responses;
global using Morsley.UK.People.API.Contracts.Shared;
global using Morsley.UK.People.API.Read.Endpoints;
global using Morsley.UK.People.API.Read.Services;
global using Morsley.UK.People.API.Security.Endpoints;
global using Morsley.UK.People.API.Security.Interfaces;
global using Morsley.UK.People.API.Security.Services;
global using Morsley.UK.People.Application.Interfaces;
global using Morsley.UK.People.Application.IoC;
global using Morsley.UK.People.Application.Queries;
global using Morsley.UK.People.Caching.IoC;
global using Morsley.UK.People.Common;
global using Morsley.UK.People.Domain.Models;
global using Morsley.UK.People.Messaging.IoC;
global using Morsley.UK.People.Persistence.IoC;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using Serilog;
global using Serilog.Events;
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Dynamic;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
global using ILogger = Serilog.ILogger;
