using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace efscaffold.Entities;

public partial class TickticktaskDto
{
    [IsGuid]
    public string TaskId { get; set; } = null!;

    [IsGuid]
    public string ListId { get; set; } = null!;

    [MinLength(1)]    
    public string Title { get; set; } = null!;

    [MinLength(1)]
    public string Description { get; set; } = null!;

    [Required]
    public DateTime DueDate { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; }

    [Required]
    public bool Completed { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime CompletedAt { get; set; }
    
    [Required]
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
