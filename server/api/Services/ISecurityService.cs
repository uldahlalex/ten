using api.Models;

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