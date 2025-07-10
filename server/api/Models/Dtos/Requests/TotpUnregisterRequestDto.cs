using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Unregister is basically "delete"
/// </summary>
/// <param name="TotpCode">The TOTP code to verify before unregistering</param>
public record TotpUnregisterRequestDto(
    [StringLength(6, MinimumLength = 6), RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")] string TotpCode
);