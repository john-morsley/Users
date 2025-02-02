﻿global using Microsoft.AspNetCore.Mvc;
global using Morsley.UK.People.API.Contracts.Constants;
global using Morsley.UK.People.API.Contracts.Requests;
global using Morsley.UK.People.API.Contracts.Responses;
global using Morsley.UK.People.API.Contracts.Shared;
global using Morsley.UK.People.API.Example.MVC.Helpers;
global using Morsley.UK.People.API.Example.MVC.Models;
global using Morsley.UK.People.API.MVC.Models;
global using Morsley.UK.People.Common;
global using Morsley.UK.People.Domain.Enumerations;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using Serilog;
global using Serilog.Events;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Globalization;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using ILogger = Serilog.ILogger;
