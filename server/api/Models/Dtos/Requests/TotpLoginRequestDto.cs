using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Login is when the 6 digit code is sent to the server
/// </summary>
/// <param name="TotpCode">This code is found in the authenticator on the device</param>
/// <param name="Email">Email is relevant because backend needs a unique identifier to make a lookup</param>
public record TotpLoginRequestDto(
    [Required, StringLength(6, MinimumLength = 6)] string TotpCode,
    [Required, EmailAddress] string Email
);