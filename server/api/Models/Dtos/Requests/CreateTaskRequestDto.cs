using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class CreateTaskRequestDto
{
    [Required] public string ListId { get; set; } = null!;

    [MinLength(1)] [Required] public string Title { get; set; } = null!;

    [MinLength(1)] [Required] public string Description { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    [Range(1, 5)] [Required] public int Priority { get; set; }

    public virtual ICollection<TaskTagDto> TaskTagsDtos { get; set; } = new List<TaskTagDto>();
}