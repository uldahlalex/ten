using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
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
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Login using John's credentials from TestDataSeeder
        var dto = new AuthRequestDto("john@example.com", "password");

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Login), dto);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
            
        var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
        
        if (jwt == null)
            throw new Exception("Response body was null when deserializing to JwtResponse");
            
        var jwtService = _scopedServiceProvider.GetRequiredService<IJwtService>();
        var userService = _scopedServiceProvider.GetRequiredService<IUserDataService>();
        
        var claims = jwtService.VerifyJwt(jwt.Jwt); // throws if JWT is invalid
        
        // Verify the JWT contains John's ID from test data
        if (claims.Id != ids.JohnId)
            throw new Exception($"Expected JWT to contain John's ID {ids.JohnId} but got {claims.Id}");
            
        if (!await userService.UserExistsAsync(claims.Id))
            throw new Exception($"User with ID {claims.Id} should exist in database");
    }
}