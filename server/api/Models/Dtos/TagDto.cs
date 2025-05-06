using System.ComponentModel.DataAnnotations;

namespace efscaffold.Entities;

public class TagDto
{
    [Required]
    public string TagId { get; set; } = null!;
    [Required]

    public string Name { get; set; } = null!;
    [Required]

    public string UserId { get; set; } = null!;
    [Required]

    public DateTime CreatedAt { get; set; }
}