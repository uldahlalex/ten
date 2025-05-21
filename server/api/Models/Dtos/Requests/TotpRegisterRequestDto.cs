using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

/// <summary>
///     When register is performed the client app reveals the QR code
/// </summary>
public class TotpRegisterRequestDto
{
    public TotpRegisterRequestDto(string email)
    {
        Email = email;
    }

    /// <summary>
    ///     TOTP required unique identifier for lookup: Email can be used for this
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}