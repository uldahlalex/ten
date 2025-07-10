using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Replaces all of the properties with the values. Nulls are not allowed, since the client app should send the
///     existing object and not just declare certain properties to replace
/// </summary>
/// <param name="Id">The unique identifier of the task to update</param>
/// <param name="ListId">The ID of the list this task belongs to</param>
/// <param name="Completed">Whether the task is completed</param>
/// <param name="Title">The updated title of the task</param>
/// <param name="Description">The updated description of the task</param>
/// <param name="DueDate">Due date can be "removed" by assigning it null</param>
/// <param name="Priority">Priority level from 1 to 5</param>
public record UpdateTaskRequestDto(
    string Id,
    string ListId,
    bool Completed,
    [MinLength(1)] string Title,
    [MinLength(1)] string Description,
    DateTime? DueDate,
    [Range(1, 5)] int Priority
);