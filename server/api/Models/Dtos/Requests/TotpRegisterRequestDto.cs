using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     When register is performed the client app reveals the QR code
/// </summary>
public record TotpRegisterRequestDto(
    
    /// <summary>
    ///     TOTP required unique identifier for lookup: Email can be used for this
    /// </summary>
    [Required, EmailAddress] string Email
);