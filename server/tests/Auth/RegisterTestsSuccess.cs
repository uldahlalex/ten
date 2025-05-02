using System.Net.Http.Json;
using api;
using api.Seeder;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace tests.Auth;

[TestFixture]
public class RegisterTestsSuccess
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
        builder.DefaultTestConfig();
        
        _app = builder.Build();
         Program.ConfigureApp(_app);
        await _app.StartAsync();
        
        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = new HttpClient();
        await _client.TestRegisterAndAddJwt(_baseUrl);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _client.Dispose();
   
        await _app.StopAsync();
        await _app.DisposeAsync();
        
    }





    [Test]
    public async Task WhenUserRegistersWithValidCredentials_TheyGetValidJwtBack()
    {
        var reqDto = new AuthRequestDto
        {
            Email = new Random().NextDouble() * 100 + "@email.com",
            Password = new Random().NextDouble() * 10293809213 + ""
        };
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Register), reqDto);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Did not get success status code. " +
                                $"Status code: {response.StatusCode}, " +
                                $"Response: {await response.Content.ReadAsStringAsync()}");

        var jwt = await response.Content.ReadAsStringAsync();
        _scopedServiceProvider.GetRequiredService<ISecurityService>()
            .VerifyJwtOrThrow(jwt); //throws if JWT issued is invalid
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>().Users.First(u => u.Email == reqDto.Email); //throws if not found

    }
}