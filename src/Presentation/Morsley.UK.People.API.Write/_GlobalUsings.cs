﻿global using AutoMapper;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.JsonPatch;
global using Microsoft.AspNetCore.JsonPatch.Converters;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.AspNetCore.Mvc.Infrastructure;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Morsley.UK.People.API.Common.Exceptions;
global using Morsley.UK.People.API.Common.Helpers;
global using Morsley.UK.People.API.Common.Swagger;
global using Morsley.UK.People.API.Contracts.Extensions;
global using Morsley.UK.People.API.Contracts.IoC;
global using Morsley.UK.People.API.Contracts.Requests;
global using Morsley.UK.People.API.Contracts.Responses;
global using Morsley.UK.People.API.Contracts.Shared;
global using Morsley.UK.People.API.Write.Endpoints;
global using Morsley.UK.People.Application.Commands;
global using Morsley.UK.People.Application.IoC;
global using Morsley.UK.People.Application.Queries;
global using Morsley.UK.People.Common;
global using Morsley.UK.People.Domain.Models;
global using Morsley.UK.People.Persistence.IoC;
global using Serilog;
global using Serilog.Events;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
global using ILogger = Serilog.ILogger;