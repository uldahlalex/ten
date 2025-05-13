using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using api.Models;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace api.Services;

public interface ISecurityService
{
    string GenerateJwt(string id);
    JwtClaims VerifyJwtOrThrow(string jwt);
    string Hash(string str);
    string GenerateQrCodeBase64(string otpauthUrl);
    string? GenerateSecretKey();
    void ValidateTotpCodeOrThrow(string? userTotpSecret, string requestTotpCode);
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

    public string Hash(string str)
    {
        return Convert.ToBase64String(SHA512.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(str)));
    }

    public string GenerateQrCodeBase64(string otpauthUrl)
    {
        throw new NotImplementedException();
    }

    public string? GenerateSecretKey()
    {
        throw new NotImplementedException();
    }

    public void ValidateTotpCodeOrThrow(string? userTotpSecret, string requestTotpCode)
    {
        throw new NotImplementedException();
    }
}