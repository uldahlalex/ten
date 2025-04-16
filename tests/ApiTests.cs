using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using Startup.Tests.TestUtils;

namespace ten.tests;

[TestFixture]
public class ApiTests
    : WebApplicationFactory<Program>
{
    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;

    [SetUp]
    public void Setup()
    {
        _httpClient = CreateClient();
        _scopedServiceProvider = Services.CreateScope().ServiceProvider;
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => { services.DefaultTestConfig(); });
    }

    [Test]
    public async Task GetDeviceLogsTest()
    {
        var req = await _httpClient.GetAsync(MyControllerClass.GetDeviceLogsRoute);
        if (req.IsSuccessStatusCode)
            throw new Exception("Did not get success status code");
        
    }
    
}