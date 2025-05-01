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
    
            await _app.StopAsync();
            await _app.DisposeAsync();
        
    }

 
    [Test]
    [Explicit]
    public async Task Waits()
    {
        //Pause test indefinitely
        Console.ReadLine();
    }
}