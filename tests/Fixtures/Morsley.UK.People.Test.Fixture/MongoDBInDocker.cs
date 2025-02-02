﻿namespace Morsley.UK.People.Test.Fixture;

public class MongoDBInDocker : InDocker
{
    public const string MONGODB_IMAGE = "mongo";
    public const string MONGODB_IMAGE_TAG = "5";
    //public const string MONGODB_CONTAINER_NAME = "IntegrationTesting_MongoDB";
    public const string MONGODB_AUTHENTICATION_MECHANISM = "SCRAM-SHA-1";
    //public const int MONGODB_PORT = 27017;

    public MongoDBInDocker(
        string containerName, 
        string username, 
        string password, 
        int port) : base(username, password, port)
    {
        _containerName =containerName;
    }

    private string _containerName;

    public async override Task<(string containerId, int port)> EnsureDockerStartedAndGetContainerIdAndPortAsync()
    {
        await CleanupRunningContainers();

        var dockerClient = GetDockerClient();
        var freePort = GetFreePort();

        // This call ensures that the latest Docker image is pulled
        var imagesCreateParameters = new ImagesCreateParameters { FromImage = $"{MONGODB_IMAGE}:{MONGODB_IMAGE_TAG}" };
        await dockerClient.Images.CreateImageAsync(imagesCreateParameters, null, new Progress<JSONMessage>());

        var container = await dockerClient
            .Containers
            .CreateContainerAsync(new CreateContainerParameters
            {
                Name = _containerName,
                Image = $"{MONGODB_IMAGE}:{MONGODB_IMAGE_TAG}",
                Env = new List<string>
                {
                    $"MONGO_INITDB_ROOT_USERNAME={Username}",
                    $"MONGO_INITDB_ROOT_PASSWORD={Password}"
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            $"{Port}/tcp",
                            new PortBinding[]
                            {
                                new PortBinding
                                {
                                    HostPort = freePort.ToString(),
                                }
                            }
                        }
                    }
                },
            });

        await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
        await WaitUntilDatabaseIsAvailableAsync(_containerName, Username, Password, freePort);

        return (container.ID, freePort);
    }

    //public static async Task<string> GetPort()
    //{
    //    var dockerClient = GetDockerClient();
    //    var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
    //    //var existingContainer = await containers.Where(c => c.Names.Any(n => n.Contains(MONGODB_CONTAINER_NAME))).FirstOrDefault;
    //    //return existingContainer.Port;

    //    throw new NotImplementedException();
    //}

    private static bool IsRunningOnWindows()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    private static DockerClient GetDockerClient()
    {
        var dockerUri = IsRunningOnWindows() ? "npipe://./pipe/docker_engine" : "unix:///var/run/docker.sock";
        return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
    }

    private async Task CleanupRunningContainers()
    {
        var dockerClient = GetDockerClient();

        var runningContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });

        foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(_containerName))))
        {
            try
            {
                await EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
            }
            catch
            {
                // Ignoring failures to stop running containers
            }
        }
    }

    public async override Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Containers.StopContainerAsync(dockerContainerId, new ContainerStopParameters());
        await dockerClient.Containers.RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
    }

    public async override Task EnsureDockerVolumesRemovedAsync(string volumeName)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Volumes.RemoveAsync(volumeName);
    }

    private async static Task WaitUntilDatabaseIsAvailableAsync(string containerName, string username, string password, int databasePort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                var internalIdentity = new MongoInternalIdentity("admin", username);
                var passwordEvidence = new PasswordEvidence(password);
                var mongoCredential = new MongoCredential(MONGODB_AUTHENTICATION_MECHANISM, internalIdentity, passwordEvidence);

                var mongoClientSettings = new MongoClientSettings
                {
                    Credential = mongoCredential,
                    Server = new MongoServerAddress("localhost", databasePort)
                };

                var client = new MongoClient(mongoClientSettings);
                var instance = client.GetDatabase(containerName);

                connectionEstablished = true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                await Task.Delay(500);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to the MongoDB docker instance could not be established within {maxWaitTimeSeconds} seconds.");
        }

        return;
    }

    private static int GetFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }

    public string ConnectionString()
    {
        return $"mongodb://{Username}:{Password}@localhost:{Port}";
    }
}