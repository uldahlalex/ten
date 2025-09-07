using api.Etc;
using api.Models.Dtos.Requests;
using api.Services;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;
using Infrastructure.Postgres.Scaffolding;

namespace tests.Auth;

public class LoginTestSuccess : ApiTestBase
{


    [Test]
    public async Task Login_CanSuccessfully_Login()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var john = ScopedServiceProvider.GetRequiredService<MyDbContext>().Users.First(u => u.UserId == ids.JohnId);
        
        var dto = new AuthRequestDto(john.Email, "password");

        var jwt = (await ApiClient.Auth_LoginAsync(dto));
            
        var jwtService = ScopedServiceProvider.GetRequiredService<IJwtService>();
        var userService = ScopedServiceProvider.GetRequiredService<IUserDataService>();
        
        var claims = jwtService.VerifyJwtOrThrow(jwt.Jwt); // throws if JWT is invalid
        
        if (claims.Id != ids.JohnId)
            throw new Exception($"Expected JWT to contain John's ID {ids.JohnId} but got {claims.Id}");
            
        if (!await userService.UserExistsAsync(claims.Id))
            throw new Exception($"User with ID {claims.Id} should exist in database");
    }
}