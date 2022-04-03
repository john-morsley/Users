﻿global using FluentAssertions;
global using Microsoft.AspNetCore.JsonPatch;
global using Morsley.UK.People.API.Contracts.Requests;
global using Morsley.UK.People.API.Test.Fixture;
global using Morsley.UK.People.Application.Events;
global using Morsley.UK.People.Application.Handlers;
global using Morsley.UK.People.Domain.Enumerations;
global using Morsley.UK.People.Domain.Models;
global using Morsley.UK.People.Tests.Common;
global using Newtonsoft.Json;
global using NUnit.Framework;
global using System;
global using System.Collections.Generic;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Threading.Tasks;