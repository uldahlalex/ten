using api.Models;

namespace api.Services;

public interface IJwtService
{
    Task<string> GenerateJwt(string id);
    Task<JwtClaims> VerifyJwtOrThrow(string jwt);
}