using System.ComponentModel.DataAnnotations;

namespace efscaffold.Entities;
/// <summary>
/// Data annotations on properties are used for validation in tests
/// </summary>
public class TickticktaskDto
{
    [Required] public string TaskId { get; set; } = null!;

    [Required] public string ListId { get; set; } = null!;

    [MinLength(1)] public string Title { get; set; } = null!;

    [MinLength(1)] public string Description { get; set; } = null!;

    public DateTime ?DueDate { get; set; }

    [Range(1, 5)] public int Priority { get; set; }

    [Required] public bool Completed { get; set; }

    [Required] public DateTime CreatedAt { get; set; }

     public DateTime ?CompletedAt { get; set; }

     public virtual ICollection<TaskTagDto> TaskTags { get; set; } = new List<TaskTagDto>();
}