using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

/// <summary>
/// Login is when the 6 digit code is sent to the server
/// </summary>
public class TotpLoginRequestDto
{
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string TotpCode { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}