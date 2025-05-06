using api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests;

public class IntegrationTests
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public async System.Threading.Tasks.Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder);
        builder.DefaultTestConfig();

        _app = builder.Build();
        Program.ConfigureApp(_app);
        await _app.StartAsync();

        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = new HttpClient();
        await _client.TestRegisterAndAddJwt(_baseUrl);
    }


    [Test]
    [Explicit]
    public async System.Threading.Tasks.Task Waits()
    {
        //Pause test indefinitely
        Console.ReadLine();
    }
}