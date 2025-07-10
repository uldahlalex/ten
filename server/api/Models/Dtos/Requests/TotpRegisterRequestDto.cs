using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     When register is performed the client app reveals the QR code
/// </summary>
/// <param name="Email">TOTP required unique identifier for lookup: Email can be used for this</param>
public record TotpRegisterRequestDto(
    [Required, EmailAddress] string Email
);