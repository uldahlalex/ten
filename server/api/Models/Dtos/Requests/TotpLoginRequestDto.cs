using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

/// <summary>
/// Login is when the 6 digit code is sent to the server
/// </summary>
public class TotpLoginRequestDto
{
    public TotpLoginRequestDto(string totpCode, string email)
    {
        TotpCode = totpCode;
        Email = email;
    }

    /// <summary>
    /// This code is found in the authenticator on the device
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string TotpCode { get; set; }
    
    /// <summary>
    /// Email is relevant because backend needs a unique identifier to make a lookup
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}