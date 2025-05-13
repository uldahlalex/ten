using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpRotateRequestDto
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
    public string CurrentTotpCode { get; set; }
}