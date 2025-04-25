namespace efscaffold.Entities;

public class Tasklist
{
    public string ListId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Tickticktask> Tickticktasks { get; set; } = new List<Tickticktask>();

    public virtual User User { get; set; } = null!;
}