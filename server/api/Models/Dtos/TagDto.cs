namespace efscaffold.Entities;

public class TagDto
{
    public string TagId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}