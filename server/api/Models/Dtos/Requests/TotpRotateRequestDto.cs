using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Used to change the persisted secret to a new random one (not supplied by client)
/// </summary>
/// <param name="CurrentTotpCode">The current TOTP code to verify before rotation</param>
public record TotpRotateRequestDto(
    [StringLength(6, MinimumLength = 6), RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")] string CurrentTotpCode
);