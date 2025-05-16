using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Auth;

public class LoginTestSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public Task Setup()
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
        _client = new HttpClient(); 
        return Task.CompletedTask;
    }


    [Test]
    public async Task Login_CanSuccessfully_Login()
    {
        var dto = new AuthRequestDto
        {
            Email = "test@user.dk",
            Password = "abc"
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Login), dto);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Login failed: {response.StatusCode}");
        var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
        _scopedServiceProvider.GetRequiredService<ISecurityService>()
            .VerifyJwtOrThrow(jwt.Jwt); //throws if JWT issued is invalid
    }
}