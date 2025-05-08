using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Services;
using efscaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Auth;

public class RegisterTestsSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public async System.Threading.Tasks.Task Setup()
    {
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting();
        builder.AddProgramcsServices();
        builder.ModifyServicesForTesting();
        _app = builder.Build();

        _app.BeforeProgramcsMiddleware();
        _app.AddProgramcsMiddleware();
        _app.AfterProgramcsMiddleware();
        
        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = new HttpClient(); //should not use the method which adds jwt

    }

    [Test]
    public async System.Threading.Tasks.Task WhenUserRegistersWithValidCredentials_TheyGetValidJwtBack()
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
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>().Users
            .First(u => u.Email == reqDto.Email); //throws if not found
    }
}