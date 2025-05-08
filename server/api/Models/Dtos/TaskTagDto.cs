using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos;

public class TaskTagDto
{
    [Required]
    public string TaskId { get; set; } = null!;

    [Required]
    public string TagId { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }
}