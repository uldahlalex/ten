using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Text.Json;
using api.Models;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace api.Services;

public class JwtService(AppOptions appOptions) : IJwtService
{
    public async Task<string> GenerateJwt(string id)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(appOptions.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .AddClaim(nameof(id), id)
            .Encode();
    }

    public async Task<JwtClaims> VerifyJwtOrThrow(string jwt)
    {
        if (string.IsNullOrEmpty(jwt))
            throw new AuthenticationException("No JWT attached!");
        var claims = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(appOptions.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .MustVerifySignature().Decode<JwtClaims>(jwt);
        
        Validator.ValidateObject(claims, new ValidationContext(claims), true);
        return claims;
    }
}