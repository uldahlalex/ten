using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

/// <summary>
///     Unregister is basically "delete"
/// </summary>
public class TotpUnregisterRequestDto
{
    public TotpUnregisterRequestDto(string totpCode)
    {
        TotpCode = totpCode;
    }

    [Required]
    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
    public string TotpCode { get; set; }
}