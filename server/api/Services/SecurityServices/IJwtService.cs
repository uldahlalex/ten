using api.Models;

namespace api.Services;

public interface IJwtService
{
    string GenerateJwt(string id, string jwtSecret);
    JwtClaims VerifyJwt(string jwt);
}