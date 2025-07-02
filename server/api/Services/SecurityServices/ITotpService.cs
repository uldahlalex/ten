namespace api.Services;

public interface ITotpService
{
    string GenerateSecretKey();
    string GenerateQrCodeBase64(string otpauthUrl);
    void ValidateTotpCodeOrThrow(string? userTotpSecret, string requestTotpCode);
}