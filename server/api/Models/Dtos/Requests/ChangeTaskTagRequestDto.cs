using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Used for assigning and de-assigning tags to tasks. Works as "toggle", so if the tag already exists it is removed
///     and vice versa.
/// </summary>
/// <param name="TagId">The unique identifier of the tag to toggle</param>
/// <param name="TaskId">The unique identifier of the task to toggle the tag on</param>
public record ChangeTaskTagRequestDto(
    string TagId,
    string TaskId
);