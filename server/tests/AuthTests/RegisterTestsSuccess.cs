using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
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
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        
        // Create unique credentials for this test
        var uniqueEmail = $"testuser_{Guid.NewGuid():N}@example.com";
        var password = "TestPassword123!";
        var reqDto = new AuthRequestDto(uniqueEmail, password);
        
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(AuthController.Register), reqDto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Expected success status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");

        var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
        
        if (jwt == null)
            throw new Exception("Response body was null when deserializing to JwtResponse");
            
        var jwtService = _scopedServiceProvider.GetRequiredService<IJwtService>();
        var userService = _scopedServiceProvider.GetRequiredService<IUserDataService>();
        
        var claims = jwtService.VerifyJwt(jwt.Jwt); // throws if JWT is invalid
        
        if (string.IsNullOrEmpty(claims.Id))
            throw new Exception("JWT claims should contain a valid user ID");
            
        if (!await userService.UserExistsAsync(claims.Id))
            throw new Exception($"User with ID {claims.Id} should exist in database after registration");
            
        // Verify the user was created in database with correct email
        var createdUser = ctx.Users.FirstOrDefault(u => u.Email == reqDto.Email);
        if (createdUser == null)
            throw new Exception($"User with email {reqDto.Email} should exist in database after registration");
            
        if (createdUser.UserId != claims.Id)
            throw new Exception($"User ID in database {createdUser.UserId} should match JWT claims {claims.Id}");
    }
}