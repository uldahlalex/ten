using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Used to verify a TOTP code for a specific user
/// </summary>
/// <param name="Id">The unique identifier of the user to verify</param>
/// <param name="TotpCode">The TOTP code to verify</param>
public record TotpVerifyRequestDto(
    string Id,
    [StringLength(6, MinimumLength = 6), RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")] string TotpCode
);