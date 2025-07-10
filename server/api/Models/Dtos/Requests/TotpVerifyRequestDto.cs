using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public record TotpVerifyRequestDto(
    [Required] string Id,
    [Required, StringLength(6, MinimumLength = 6), RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")] string TotpCode
);