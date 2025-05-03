using System.ComponentModel.DataAnnotations;
using efscaffold.Entities;

namespace api;

public class CreateTaskRequestDto
{
    public string ListId { get; set; } = null!;

    [MinLength(1)] public string Title { get; set; } = null!;

    [MinLength(1)] public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    [Range(1, 5)] public int Priority { get; set; }

    public virtual ICollection<TaskTagDto> TaskTagsDtos { get; set; } = new List<TaskTagDto>();
}