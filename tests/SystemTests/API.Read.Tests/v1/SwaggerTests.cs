namespace Users.API.Read.Tests.v1;

public class SwaggerTests
{
    private TestServer _server;
    private HttpClient _client;
        
    [SetUp]
    public void Setup()
    {
        var webHostBuilder = new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
            })
            .UseEnvironment("Development")
            .UseStartup<StartUp>();
        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();
    }

    [Test]
    public async Task The_Base_URL_Should_Result_In_A_Redirect_To_The_Swagger_Endpoint()
    {
        // Arrange...
        const string url = "";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Moved);
        httpResponse.Headers.Location.Should().Be("index.html");
    }
        
    [Test]
    public async Task The_Swagger_Endpoint_Should_Result_In_The_Swagger_Page()
    {
        // Arrange...
        const string url = "/index.html";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(response);
        var title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;
        title.Should().Be("Swagger UI");
    }

    [Test]
    public async Task The_Endpoint_For_The_Swagger_JSON_Should_Yield_OpenAPI_JSON()
    {
        // Arrange...
        const string url = "/swagger/v1/swagger.json";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        //var swagger = JObject.Parse(response);
        //swagger.Count.Should().Be(4);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}