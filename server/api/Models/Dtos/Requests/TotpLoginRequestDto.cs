using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpLoginRequestDto
{
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string TotpCode { get; set; }
}