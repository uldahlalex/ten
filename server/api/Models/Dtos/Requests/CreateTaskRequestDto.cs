using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Task is always created for the user sending the request
/// </summary>
/// <param name="ListId">The ID of the list to create the task in</param>
/// <param name="Title">The title of the task (minimum 1 character)</param>
/// <param name="Description">The description of the task (minimum 1 character)</param>
/// <param name="DueDate">Due date is optional since tasks may have none</param>
/// <param name="Priority">Priority level from 1 to 5</param>
/// <param name="TagsIds">List of tag IDs to add to the task when it is created</param>
public record CreateTaskRequestDto(
    string ListId,
    [MinLength(1)] string Title,
    [MinLength(1)] string Description,
    DateTime? DueDate,
    [Range(1, 5)] int Priority,
    ICollection<string> TagsIds
);