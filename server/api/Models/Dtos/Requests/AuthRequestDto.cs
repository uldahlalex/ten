using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
/// Used both for sign in and registration. Password repeat verified client side
/// </summary>
public class AuthRequestDto
{
    public AuthRequestDto(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public AuthRequestDto(string email)
    {
        Email = email;
    }

    /// <summary>
    /// Just to have any unique identifier for the user when signing in and registering
    /// </summary>
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}