namespace efscaffold.Entities;

public class Tickticktask
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

    public virtual Tasklist List { get; set; } = null!;

    public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}