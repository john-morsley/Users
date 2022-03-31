﻿namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Write application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker RabbitMQ instance for the SUT
// 4. Creating a Docker MongoDB instance for the SUT
public class WriteApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public BusTestFixture BusTestFixture => _busTestFixture!;

    public DatabaseTestFixture ApplicationDatabase => _writeDatabaseTestFixture!;

    protected BusTestFixture? _busTestFixture;

    protected DatabaseTestFixture? _readDatabaseTestFixture;

    protected DatabaseTestFixture? _writeDatabaseTestFixture;

    protected WorkerTestFixture<SynchronizerProgram> _synchronizerTestFixture;

    public WriteApplicationTestFixture()
    {

    }

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        var busConfiguration = GetBusConfiguration();
        _busTestFixture = new BusTestFixture("Bus_Test", busConfiguration, "ReadMongoDBSettings");
        await _busTestFixture.CreateBus();

        var readDatabaseConfiguration = GetReadDatabaseConfiguration();
        _readDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test", readDatabaseConfiguration, "ReadMongoDBSettings");
        await _readDatabaseTestFixture.CreateDatabase();

        var writeDatabaseConfiguration = GetWriteDatabaseConfiguration();
        _writeDatabaseTestFixture = new DatabaseTestFixture("Write_Database_Test", writeDatabaseConfiguration, "WriteMongoDBSettings");
        await _writeDatabaseTestFixture.CreateDatabase();

        var workerConfiguration = GetWorkerConfiguration(busConfiguration, readDatabaseConfiguration);
        _synchronizerTestFixture = new WorkerTestFixture<SynchronizerProgram>(workerConfiguration, "ReadMongoDBSettings");
    }

    private IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        return configuration;
    }

    private IConfiguration GetBusConfiguration()
    {
        //var all = GetConfiguration();

        //var section = all.GetSection("RabbitMQSettings");

        //var builder = new ConfigurationBuilder();

        //var settings = new Dictionary<string, string>();

        //settings.Add("RabbitMQSettings:Host", section["Host"]);
        //settings.Add("RabbitMQSettings:Port", section["Port"]);
        //settings.Add("RabbitMQSettings:Username", section["Username"]);
        //settings.Add("RabbitMQSettings:Password", section["Password"]);

        //builder.AddInMemoryCollection(settings);

        //var configuration = builder.Build();

        //return configuration;

        return GetConfiguration();
    }

    private IConfiguration GetReadDatabaseConfiguration()
    {
        //var all = GetConfiguration();

        //var section = all.GetSection("ReadMongoDBSettings");

        //var builder = new ConfigurationBuilder();

        //var settings = new Dictionary<string, string>();

        //settings.Add("MongoDBSettings:Host", section["Host"]);
        //settings.Add("MongoDBSettings:Port", section["Port"]);
        //settings.Add("MongoDBSettings:Username", section["Username"]);
        //settings.Add("MongoDBSettings:Password", section["Password"]);
        //settings.Add("MongoDBSettings:DatabaseName", section["DatabaseName"]);
        //settings.Add("MongoDBSettings:TableName", section["TableName"]);

        //builder.AddInMemoryCollection(settings);

        //var configuration = builder.Build();

        //return configuration;

        return GetConfiguration();
    }

    private IConfiguration GetWriteDatabaseConfiguration()
    {
        //var all = GetConfiguration();

        //var section = all.GetSection("WriteMongoDBSettings");

        //var builder = new ConfigurationBuilder();

        //var settings = new Dictionary<string, string>();

        //settings.Add("MongoDBSettings:Host", section["Host"]);
        //settings.Add("MongoDBSettings:Port", section["Port"]);
        //settings.Add("MongoDBSettings:Username", section["Username"]);
        //settings.Add("MongoDBSettings:Password", section["Password"]);
        //settings.Add("MongoDBSettings:DatabaseName", section["DatabaseName"]);
        //settings.Add("MongoDBSettings:TableName", section["TableName"]);

        //builder.AddInMemoryCollection(settings);

        //var configuration = builder.Build();

        //return configuration;

        return GetConfiguration();
    }

    private IConfiguration GetWorkerConfiguration(IConfiguration busConfiguration, IConfiguration  readDatabaseConfiguration)
    {
        var builder = new ConfigurationBuilder()
            .AddConfiguration(busConfiguration)
            .AddConfiguration(readDatabaseConfiguration);

        var busInMemoryConfiguration = _busTestFixture!.GetInMemoryConfiguration();
        var readDatabaseInMemoryConfiguration = _readDatabaseTestFixture!.GetInMemoryConfiguration();

        builder.AddInMemoryCollection(busInMemoryConfiguration);
        builder.AddInMemoryCollection(readDatabaseInMemoryConfiguration);

        var configuration = builder.Build();

        return configuration;
    }

    [SetUp]
    protected virtual void SetUp()
    {
        _busTestFixture!.SetUp();
        _readDatabaseTestFixture!.SetUp();
        _writeDatabaseTestFixture!.SetUp();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        _busTestFixture?.TearDown();
        _readDatabaseTestFixture?.TearDown();
        _writeDatabaseTestFixture?.TearDown();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        await _busTestFixture!.OneTimeTearDown();
        await _readDatabaseTestFixture!.OneTimeTearDown();
        await _writeDatabaseTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalBusConfiguration in _busTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
        }

        foreach (var additionalDatabaseConfiguration in _writeDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }
}
