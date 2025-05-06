using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos;

public class UserDto
{
    [Required] public string UserId { get; set; } = null!;

    [Required] public string Email { get; set; } = null!;

    [Required] public string Salt { get; set; } = null!;

    [Required] public string PasswordHash { get; set; } = null!;

    [Required] public string Role { get; set; } = null!;

    [Required] public DateTime CreatedAt { get; set; }

    [Required] public virtual ICollection<TagDto> Tags { get; set; } = new List<TagDto>();

    [Required] public virtual ICollection<TasklistDto> Tasklists { get; set; } = new List<TasklistDto>();
}