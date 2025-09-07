using api.Models;

namespace api.Services;

public interface IJwtService
{
    string GenerateJwt(string id);
    JwtClaims VerifyJwt(string jwt);
}