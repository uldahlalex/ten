using efscaffold.Entities;

namespace api;

public class CreateTaskRequestDto
{
    public string ListId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public int Priority { get; set; }
    
    public virtual ICollection<TaskTagDto> TaskTagsDtos { get; set; } = new List<TaskTagDto>();
    

}