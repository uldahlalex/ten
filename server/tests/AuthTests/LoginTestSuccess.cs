using api.Etc;
using api.Models.Dtos.Requests;
using api.Services;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.Auth;

public class LoginTestSuccess : ApiTestBase
{
    protected override Task OnSetupComplete()
    {
        // Login test doesn't need the default authenticated client
        // Use an unauthenticated client instead
        Client?.Dispose();
        Client = new HttpClient();
        
        var baseUrl = App.Urls.First() + "/";
        ApiClient = new ApiClient(baseUrl, Client);
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task Login_CanSuccessfully_Login()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Login using John's credentials from TestDataSeeder
        var dto = new AuthRequestDto("john@example.com", "password");

        var jwt = await ApiClient.Auth_LoginAsync(dto);
            
        var jwtService = ScopedServiceProvider.GetRequiredService<IJwtService>();
        var userService = ScopedServiceProvider.GetRequiredService<IUserDataService>();
        
        var claims = jwtService.VerifyJwt(jwt.Jwt); // throws if JWT is invalid
        
        // Verify the JWT contains John's ID from test data
        if (claims.Id != ids.JohnId)
            throw new Exception($"Expected JWT to contain John's ID {ids.JohnId} but got {claims.Id}");
            
        if (!await userService.UserExistsAsync(claims.Id))
            throw new Exception($"User with ID {claims.Id} should exist in database");
    }
}