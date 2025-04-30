using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;
    private string _baseUrl = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        
        Program.ConfigureServices(builder);
        
        _app = builder.Build();
        Program.ConfigureApp(_app);
        
        await _app.StartAsync();
        _baseUrl = _app.Urls.First();
        Console.WriteLine($"Test API running at: {_baseUrl}");
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;

        
        _client = new HttpClient();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _client.Dispose();
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    [Test]
    public async Task HelloWorld_Returns200()
    {
        // Arrange & Act
        var response = await _client.GetAsync($"{_baseUrl}/helloworld");
        //Pause test indefinitely
        Console.ReadLine();
        // Assert
        Assert.That(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("Hello World!"));
    }
    [Test]
    [Explicit]
    public async Task Waits()
    {
        //Pause test indefinitely
        Console.ReadLine();
    }
}