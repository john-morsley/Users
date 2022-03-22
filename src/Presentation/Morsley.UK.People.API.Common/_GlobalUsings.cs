﻿global using AutoMapper;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.PlatformAbstractions;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Morsley.UK.People.API.Contracts.Extensions;
global using Morsley.UK.People.API.Contracts.Requests;
global using Morsley.UK.People.API.Contracts.Responses;
global using Morsley.UK.People.Application.Commands;
global using Morsley.UK.People.Application.Queries;
global using Morsley.UK.People.Domain.Models;
global using Serilog;
global using System;
global using System.Collections.Generic;
global using System.Dynamic;
global using System.IO;
global using System.Text;
global using System.Threading.Tasks;
