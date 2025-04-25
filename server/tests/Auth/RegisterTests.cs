using System.Text;
using System.Text.Json;
using api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace tests.Auth;

[TestFixture]
public class RegisterTests
{
    
    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;

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


    [Test]
    public async Task WhenUserRegistersWithValidCredentials_TheyGetValidJwtBack()
    {
        var reqDto = new AuthRequestDto()
        {
            Email = new Random().NextDouble() * 100 + "@email.com",
            Password = new Random().NextDouble() * 10293809213 + ""
        };
        var response = await _httpClient.PostAsync(AuthController.RegisterRoute
            , new StringContent(JsonSerializer.Serialize(reqDto), Encoding.UTF8, "application/json"));
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Did not get success status code");

        var jwt = await response.Content.ReadAsStringAsync();
        _scopedServiceProvider.GetRequiredService<ISecurityService>().VerifyJwtOrThrow(jwt); //throws if JWT issued is invalid
    }
    
}