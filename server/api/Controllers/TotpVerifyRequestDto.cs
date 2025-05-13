using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpVerifyRequestDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
    public string TotpCode { get; set; }
}