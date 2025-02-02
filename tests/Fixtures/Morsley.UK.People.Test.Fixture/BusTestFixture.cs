﻿namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class BusTestFixture
{
    protected DockerTestFixture<RabbitMQInDocker>? DockerTestFixture;
    //protected global::AutoFixture.Fixture? AutoFixture;

    private IConfiguration? _configuration;

    public IConfiguration? Configuration => _configuration;

    private readonly string _messagingKey;
    private readonly string _persistenceKey;

    private IEventBus? _eventBus;

    private string _name;

    public BusTestFixture(string name, IConfiguration configuration, string messagingKey, string persistenceKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");
        if (configuration == null) throw new ArgumentNullException("configuration");

        //AutoFixture = new global::AutoFixture.Fixture();
        //AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        //AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        //_eventBus = new EventBus();

        _name = name;
        _configuration = configuration;
        _messagingKey = messagingKey;
        _persistenceKey = persistenceKey;
    }

    public async Task CreateBus()
    {
        //LoadInitialConfiguration();

        //var section = _configuration!.GetSection(nameof(RabbitMQSettings));
        //var settings = section.Get<RabbitMQSettings>();

        //var settings = _configuration["RabbitMQSettings"];

        var potentialPort = _configuration[$"{_messagingKey}:Port"];
        var username = _configuration[$"{_messagingKey}:Username"];
        var password = _configuration[$"{_messagingKey}:Password"];

        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<RabbitMQInDocker>(_name, username!, password!, port);

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }

        UpdateConfiguration();

        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            //services.AddSingleton<IEventBus, EventBus>();
            services.AddScoped<PersonAddedEventHandler>();
            services.AddScoped<PersonDeletedEventHandler>();
            services.AddScoped<PersonUpdatedEventHandler>();
            //services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddApplication();
            services.AddPersistence(_configuration, _persistenceKey);
        });

        var host = builder.Build();

        var factory = host.Services.GetService<IServiceScopeFactory>();
        
        _eventBus = new EventBus(_configuration, factory!, null);
    }

    private void UpdateConfiguration()
    {
        //var builder = new ConfigurationBuilder().AddConfiguration(_configuration);
        //builder.AddInMemoryCollection(GetInMemoryConfiguration());
        //var configuration = builder.Build();

        var configurator = new Configurator();
        var extra = GetInMemoryConfiguration();
        configurator.AddConfiguration(_configuration);
        configurator.AddConfiguration(extra);
        var configuration = configurator.Build();


        var potentialPort = configuration["RabbitMQSettings:Port"];
        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }
        if (port != DockerTestFixture!.InDocker.Port)
        {
            throw new NotImplementedException("Port has not been updated!");
        }
        _configuration = configuration;
    }

    [SetUp]
    public async virtual Task SetUp()
    {
        await Task.CompletedTask;
    }

    [TearDown]
    public async virtual Task TearDown()
    {
        await Task.CompletedTask;
    }

    [OneTimeTearDown]
    public async virtual Task OneTimeTearDown()
    {
        await DockerTestFixture!.RunAfterTests();
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        //var eventName = typeof(T).Name;
        //var handlerType = typeof(TH);
        _eventBus!.Subscribe<T, TH>();
    }

    protected int ContainerPort
    {
        get
        {
            if (DockerTestFixture == null) throw new InvalidOperationException("Docker has not yet been initialised!");
            return DockerTestFixture!.GetContainerPort();
        }
    }

    //protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    //{
    //    var builder = new ConfigurationBuilder();

    //    builder.AddJsonFile("appsettings.json");

    //    if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

    //    IConfiguration configuration = builder.Build();

    //    return configuration;
    //}

    public Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        if (DockerTestFixture != null) additional.Add("RabbitMQSettings:Port", ContainerPort.ToString());
        return additional;
    }

    //private void LoadAdditionalConfiguration()
    //{
        //var additionalConfiguration = GetInMemoryConfiguration();
        //_configuration = GetConfiguration(additionalConfiguration);
        //var builder = new ConfigurationBuilder();
        //builder.
        //builder.AddInMemoryCollection(additionalConfiguration);
        //_configuration = builder.Build();
        //var manager = new ConfigurationManager();
        //manager.
    //}

    //protected void LoadInitialConfiguration()
    //{
    //    Configuration = GetConfiguration();
    //}
}
