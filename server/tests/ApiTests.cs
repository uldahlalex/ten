using api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace tests;

[TestFixture]
public class ApiTests
{
    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => { services.DefaultTestConfig(); });
            });

        _httpClient = factory.CreateClient();
        _scopedServiceProvider = factory.Services.CreateScope().ServiceProvider;
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;
    
    [Test]
    public async Task GetDeviceLogsTest()
    {
        var req = await _httpClient.GetAsync(MyControllerClass.GetDeviceLogsRoute);
        if (req.IsSuccessStatusCode)
            throw new Exception("Did not get success status code");
        
    }
    
}