using api.Models.Dtos.Requests;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.Auth;

public class RegisterTestsSuccess : ApiTestBase
{
    protected override Task OnSetupComplete()
    {
        // Register test doesn't need the default authenticated client
        // Use an unauthenticated client instead
        Client?.Dispose();
        Client = new HttpClient();
        
        var baseUrl = App.Urls.First() + "/";
        ApiClient = new ApiClient(baseUrl, Client);
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task WhenUserRegistersWithValidCredentials_TheyGetValidJwtBack()
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        
        // Create unique credentials for this test
        var uniqueEmail = $"testuser_{Guid.NewGuid():N}@example.com";
        var password = "TestPassword123!";
        var reqDto = new AuthRequestDto(uniqueEmail, password);
        
        var jwt = await ApiClient.Auth_RegisterAsync(reqDto);
            
        var jwtService = ScopedServiceProvider.GetRequiredService<IJwtService>();
        var userService = ScopedServiceProvider.GetRequiredService<IUserDataService>();
        
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