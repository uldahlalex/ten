namespace api.Models.Dtos.Responses;

/// <summary>
///     Represents a task with all its properties and associated tags
/// </summary>
/// <param name="TaskId">Unique identifier for the task</param>
/// <param name="ListId">ID of the list this task belongs to</param>
/// <param name="Title">The task title</param>
/// <param name="Description">The task description</param>
/// <param name="DueDate">Optional due date for the task</param>
/// <param name="Priority">Priority level from 1 to 5</param>
/// <param name="Completed">Whether the task is completed</param>
/// <param name="CreatedAt">When the task was created</param>
/// <param name="CompletedAt">When the task was completed (null if not completed)</param>
/// <param name="TaskTags">Collection of tags associated with this task</param>
public record TickticktaskDto(
    string TaskId,
    string ListId,
    string Title,
    string Description,
    DateTime? DueDate,
    int Priority,
    bool Completed,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    ICollection<TaskTagDto> TaskTags
);