using System.ComponentModel.DataAnnotations;
using api.Models;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace api.Services;

public class JwtService(IOptionsMonitor<AppOptions> optionsMonitor) : IJwtService
{
    public string GenerateJwt(string id, string jwtSecret)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(jwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .AddClaim(nameof(id), id)
            .Encode();
    }

    public JwtClaims VerifyJwt(string jwt)
    {
        if (string.IsNullOrEmpty(jwt))
            throw new Exception("No JWT attached!");
        
        var claims = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .MustVerifySignature().Decode<JwtClaims>(jwt);
        
        Validator.ValidateObject(claims, new ValidationContext(claims), true);
        return claims;
    }
}