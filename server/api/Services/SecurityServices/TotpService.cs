using System.Security.Cryptography;
using OtpNet;
using QRCoder;

namespace api.Services;

public class TotpService : ITotpService
{
    public string GenerateSecretKey()
    {
        // Generate a random secret key (20 bytes is recommended for TOTP)
        var secretBytes = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(secretBytes);
        }

        // Convert to Base32 string (which is standard for TOTP)
        return Base32Encoding.ToString(secretBytes);
    }

    public string GenerateQrCodeBase64(string otpauthUrl)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(otpauthUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }

    public void ValidateTotpCodeOrThrow(string? userTotpSecret, string requestTotpCode)
    {
        if (string.IsNullOrEmpty(userTotpSecret))
            throw new Exception("TOTP secret not set for user");

        if (string.IsNullOrEmpty(requestTotpCode) || requestTotpCode.Length != 6)
            throw new Exception("Invalid TOTP code format");

        try
        {
            var totp = new Totp(Base32Encoding.ToBytes(userTotpSecret));

            var isValid = totp.VerifyTotp(requestTotpCode, out _, new VerificationWindow(1, 1));

            if (!isValid)
                throw new Exception("Invalid TOTP code");
        }
        catch (Exception ex) when (ex.Message != "Invalid TOTP code")
        {
            // Handle base32 decode errors or other issues
            throw new Exception("Invalid TOTP secret format");
        }
    }
}