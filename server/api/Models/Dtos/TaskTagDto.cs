namespace efscaffold.Entities;

public class TaskTagDto
{
    
    public string TaskId { get; set; } = null!;

    public string TagId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}