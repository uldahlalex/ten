using System.ComponentModel.DataAnnotations;
using efscaffold.Entities;

namespace api;

public class UpdateTaskRequestDto
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string ListId { get; set; } = null!;
    [Required]
    public bool Completed { get; set; } = false;

    [MinLength(1)][Required] public string Title { get; set; } = null!;

    [MinLength(1)] [Required]public string Description { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    [Range(1, 5)] public int Priority { get; set; }

}