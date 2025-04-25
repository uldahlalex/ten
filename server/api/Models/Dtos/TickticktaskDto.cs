using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class TickticktaskDto
{
    public string TaskId { get; set; } = null!;

    public string ListId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public int Priority { get; set; }

    public bool Completed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime CompletedAt { get; set; }
    
    public virtual ICollection<TaskTagDto> TaskTags { get; set; } = new List<TaskTagDto>();
    
    public TickticktaskDto FromEntity(Tickticktask tickticktask)
    {
        var dto = new TickticktaskDto
        {
            TaskId = tickticktask.TaskId,
            ListId = tickticktask.ListId,
            Title = tickticktask.Title,
            Description = tickticktask.Description,
            DueDate = tickticktask.DueDate,
            Priority = tickticktask.Priority,
            Completed = tickticktask.Completed,
            CreatedAt = tickticktask.CreatedAt,
            CompletedAt = tickticktask.CompletedAt,
            TaskTags = tickticktask.TaskTags.Select(t => new TaskTagDto().FromEntity(t)).ToList()
            
        };
        return dto;
    }
}
