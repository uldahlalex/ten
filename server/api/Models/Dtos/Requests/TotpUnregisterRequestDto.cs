using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Unregister is basically "delete"
/// </summary>
public record TotpUnregisterRequestDto(
    [Required, StringLength(6, MinimumLength = 6), RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")] string TotpCode
);