using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

/// <summary>
///     Response containing TOTP registration details including QR code
/// </summary>
/// <param name="Message">A message describing the registration status</param>
/// <param name="QrCodeBase64">Base64 encoded QR code image for TOTP setup</param>
/// <param name="SecretKey">The secret key for TOTP (for manual entry)</param>
/// <param name="UserId">The ID of the user who registered TOTP</param>
public record TotpRegisterResponseDto(
    string Message,
    string QrCodeBase64,
    string SecretKey,
    string UserId
);