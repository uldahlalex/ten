namespace efscaffold.Entities;

public class TasklistDto
{
    public string ListId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TickticktaskDto> Tickticktasks { get; set; } = new List<TickticktaskDto>();

}