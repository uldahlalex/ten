using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpVerifyRequestDto
{
    public TotpVerifyRequestDto(string id, string totpCode)
    {
        Id = id;
        TotpCode = totpCode;
    }

    [Required] public string Id { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
    public string TotpCode { get; set; }
}