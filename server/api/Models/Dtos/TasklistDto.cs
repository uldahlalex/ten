using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class TasklistDto
{
    public string ListId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TickticktaskDto> Tickticktasks { get; set; } = new List<TickticktaskDto>();

    public virtual UserDto User { get; set; } = null!;
    
    public TasklistDto FromEntity(Tasklist tasklist)
    {
        var dto = new TasklistDto
        {
            ListId = tasklist.ListId,
            UserId = tasklist.UserId,
            Name = tasklist.Name,
            CreatedAt = tasklist.CreatedAt,
            Tickticktasks = tasklist.Tickticktasks.Select(t => new TickticktaskDto().FromEntity(t)).ToList(),
            User = new UserDto().FromEntity(tasklist.User)
        };
        return dto;
    }
}
