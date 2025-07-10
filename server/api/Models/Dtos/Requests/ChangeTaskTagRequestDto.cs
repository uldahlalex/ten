using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Used for assigning and de-assigning tags to tasks. Works as "toggle", so if the tag already exists it is removed
///     and vice versa.
/// </summary>
public record ChangeTaskTagRequestDto(
    [Required] string TagId,
    [Required] string TaskId
);