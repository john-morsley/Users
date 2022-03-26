Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Verbose()
   .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

try
{
    Log.Information("Starting WRITE web host...");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Services...

    builder.Host.UseSerilog((context, services, configuration) => configuration
           .ReadFrom.Configuration(context.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext());

    //builder.Services.AddAntiforgery(); // ???
    //builder.Services.AddMvcCore(); // ???

    builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
    builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();

    var configuration = builder.Configuration;

    builder.Services.ConfigureSwaggerWithAuthentication();

    builder.Services.Configure<JsonOptions>(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
        options.JsonSerializerOptions.Converters.Add(new PersonResourceConverter());
    });

    builder.Services.AddMessaging(configuration);
    builder.Services.AddPersistence(configuration);
    builder.Services.AddApplication();
    builder.Services.AddContracts();

    var key = Environment.GetEnvironmentVariable(Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable);
    if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException($"Missing environment variable: {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> { { "Jwt:Key", key } });

    //builder.Services.AddCors(); // ???

    builder.Services.AddAuthenticationAndAuthorization(builder.Configuration, Log.Logger);

    // Configure Application...

    var application = builder.Build();

    if (!application.Environment.IsDevelopment())
    {
        //application.UseExceptionHandler("/error");
        //applicationMapDeveloperErrorEndpoint();
    }

    application.UseSerilogRequestLogging(_ => { _.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms"; });

    application.ConfigureSwagger();

    application.UseHttpsRedirection();

    application.MapSwaggerEndpoint();

    application.MapLoginEndpoint();

    application.MapOptionsPersonEndpoint();

    application.UseAuthentication();
    application.UseAuthorization();

    application.MapAddPersonEndpoint();
    application.MapUpdatePersonEndpoint();
    application.MapPartiallyUpdatePersonEndpoint();
    application.MapDeletePersonEndpoint();

    application.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

#region System Testing

/// <summary>
/// Required for System Testing only!
/// </summary>
public partial class WriteProgram { }

#endregion System Testing