using System.ComponentModel.DataAnnotations;

namespace efscaffold.Entities;

public class TickticktaskDto
{
    [IsGuid] public string TaskId { get; set; } = null!;

    [IsGuid] public string ListId { get; set; } = null!;

    [MinLength(1)] public string Title { get; set; } = null!;

    [MinLength(1)] public string Description { get; set; } = null!;

    [Required] public DateTime DueDate { get; set; }

    [Range(1, 5)] public int Priority { get; set; }

    [Required] public bool Completed { get; set; }

    [Required] public DateTime CreatedAt { get; set; }

    [Required] public DateTime CompletedAt { get; set; }

    [Required] public virtual ICollection<TaskTagDto> TaskTags { get; set; } = new List<TaskTagDto>();
}