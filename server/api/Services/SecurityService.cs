using System.Security.Cryptography;
using System.Text;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace api;

public interface ISecurityService
{
    string GenerateJwt(string id);
    JwtClaims VerifyJwtOrThrow(string jwt);
    string Hash(string str);
}

public class SecurityService(IOptionsMonitor<AppOptions> optionsMonitor) : ISecurityService
{
    public string GenerateJwt(string id)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .AddClaim(nameof(id), id)
            .Encode();
    }


    public JwtClaims VerifyJwtOrThrow(string jwt)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .MustVerifySignature().Decode<JwtClaims>(jwt);
    }

    public string Hash(string str)
    {
        return Convert.ToBase64String(SHA512.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(str)));
    }
}