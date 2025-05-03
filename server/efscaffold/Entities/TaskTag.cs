namespace efscaffold.Entities;

public class TaskTag
{
    public string TaskId { get; set; } = null!;

    public string TagId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Tag Tag { get; set; } = null!;

    public virtual Tickticktask Task { get; set; } = null!;
}