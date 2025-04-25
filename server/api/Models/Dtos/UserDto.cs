namespace efscaffold.Entities;

public class UserDto
{
    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;


    public string Salt { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TagDto> Tags { get; set; } = new List<TagDto>();

    public virtual ICollection<TasklistDto> Tasklists { get; set; } = new List<TasklistDto>();
}