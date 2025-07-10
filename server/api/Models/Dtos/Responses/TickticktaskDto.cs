using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

/// <summary>
///     Data annotations on properties are used for validation in tests
/// </summary>
public record TickticktaskDto(
    [Required] string TaskId,
    [Required] string ListId,
    [MinLength(1), Required] string Title,
    [MinLength(1), Required] string Description,
    DateTime? DueDate,
    [Range(1, 5), Required] int Priority,
    [Required] bool Completed,
    [Required] DateTime CreatedAt,
    DateTime? CompletedAt,
    [Required] ICollection<TaskTagDto> TaskTags
);