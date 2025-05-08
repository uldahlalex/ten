using System.Net;
using System.Net.Http.Json;
using api;
using api.Controllers;
using efscaffold;
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
    public async System.Threading.Tasks.Task Login_CanSuccessfully_Login()
    {
        var dto = new AuthRequestDto()
        {
            Email = "test@user.dk",
            Password = "abc"
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Login), dto);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Login failed: {response.StatusCode}");
    }
}