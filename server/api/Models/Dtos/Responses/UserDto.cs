using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

public record UserDto(
    [Required] string UserId,
    [Required] string Email,
    [Required] string Salt,
    [Required] string PasswordHash,
    [Required] string Role,
    [Required] DateTime CreatedAt,
    [Required] ICollection<TagDto> Tags,
    [Required] ICollection<TasklistDto> Tasklists
);