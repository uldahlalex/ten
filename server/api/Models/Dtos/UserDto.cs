using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class UserDto
{
    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TagDto> Tags { get; set; } = new List<TagDto>();

    public virtual ICollection<TasklistDto> Tasklists { get; set; } = new List<TasklistDto>();
    
    public UserDto FromEntity(User user)
    {
        var dto = new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Username = user.Username,
            Salt = user.Salt,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Tags = user.Tags.Select(t => new TagDto().FromEntity(t)).ToList(),
            Tasklists = user.Tasklists.Select(t => new TasklistDto().FromEntity(t)).ToList()
        };
        return dto;
    }
}
