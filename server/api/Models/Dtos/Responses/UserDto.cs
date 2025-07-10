using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

/// <summary>
///     Represents a user with all their associated data
/// </summary>
/// <param name="UserId">Unique identifier for the user</param>
/// <param name="Email">The user's email address</param>
/// <param name="Salt">Salt used for password hashing</param>
/// <param name="PasswordHash">Hashed password</param>
/// <param name="Role">The user's role in the system</param>
/// <param name="CreatedAt">When the user account was created</param>
/// <param name="Tags">Collection of tags owned by this user</param>
/// <param name="Tasklists">Collection of task lists owned by this user</param>
public record UserDto(
    string UserId,
    string Email,
    string Salt,
    string PasswordHash,
    string Role,
    DateTime CreatedAt,
    ICollection<TagDto> Tags,
    ICollection<TasklistDto> Tasklists
);