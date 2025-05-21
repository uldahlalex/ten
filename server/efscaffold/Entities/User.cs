namespace efscaffold.Entities;

public class User
{
    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Salt { get; set; }

    public string? PasswordHash { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? TotpSecret { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Tasklist> Tasklists { get; set; } = new List<Tasklist>();
}