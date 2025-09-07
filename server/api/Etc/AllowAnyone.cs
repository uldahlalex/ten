using api.Models;
using api.Services;

namespace api.Etc;

public class AllowAnyone : IJwtService
{
    public string GenerateJwt(string id)
    {
        return "AllowAnyone";
    }

    public JwtClaims VerifyJwt(string jwt)
    {
        return new JwtClaims { Id = "AllowAnyone" };
    }
}