using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Replaces all of the properties with the values. Nulls are not allowed, since the client app should send the
///     existing object and not just declare certain properties to replace
/// </summary>
public record UpdateTaskRequestDto(
    [Required] string Id,
    [Required] string ListId,
    [Required] bool Completed,
    [MinLength(1), Required] string Title,
    [MinLength(1), Required] string Description,
    /// <summary>
    ///     Due date can be "removed" by assigning it null
    /// </summary>
    DateTime? DueDate,
    [Range(1, 5), Required] int Priority
);