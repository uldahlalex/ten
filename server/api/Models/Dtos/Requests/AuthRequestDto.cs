namespace api.Models.Dtos.Requests;

/// <summary>
///     Used both for sign in and registration. Password repeat verified client side
/// </summary>
/// <param name="Email">Just to have any unique identifier for the user when signing in and registering</param>
/// <param name="Password">User's password for authentication</param>
public record AuthRequestDto(
    string Email,
    string Password
);