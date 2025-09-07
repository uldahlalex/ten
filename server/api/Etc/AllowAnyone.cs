using api.Models;
using api.Services;

namespace api.Etc;

public class AllowAnyone : IJwtService
{

    public Task<JwtClaims> VerifyJwtOrThrow(string jwt)
    {
        throw new NotImplementedException();
    }



    Task<string> IJwtService.GenerateJwt(string id)
    {
        throw new NotImplementedException();
    }
}