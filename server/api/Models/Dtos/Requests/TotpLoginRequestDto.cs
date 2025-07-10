using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Login is when the 6 digit code is sent to the server
/// </summary>
public record TotpLoginRequestDto(
    /// <summary>
    ///     This code is found in the authenticator on the device
    /// </summary>
    [Required, StringLength(6, MinimumLength = 6)] string TotpCode,
    /// <summary>
    ///     Email is relevant because backend needs a unique identifier to make a lookup
    /// </summary>
    [Required, EmailAddress] string Email
);