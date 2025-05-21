using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold;
using Infrastructure.Postgres.Scaffolding;
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
        _client = new HttpClient(); //should not use the method which adds jwt
        return Task.CompletedTask;
    }

    [Test]
    public async Task WhenUserRegistersWithValidCredentials_TheyGetValidJwtBack()
    {
        var reqDto = new AuthRequestDto(new Random().NextDouble() * 100 + "@email.com",
            new Random().NextDouble() * 1238998213 + "");
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Register), reqDto);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Did not get success status code. " +
                                $"Status code: {response.StatusCode}, " +
                                $"Response: {await response.Content.ReadAsStringAsync()}");

        var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
        _scopedServiceProvider.GetRequiredService<ISecurityService>()
            .VerifyJwtOrThrow(jwt.Jwt); //throws if JWT issued is invalid
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>().Users
            .First(u => u.Email == reqDto.Email); //throws if not found
    }
}