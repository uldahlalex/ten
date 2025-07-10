namespace api.Models.Dtos.Responses;

/// <summary>
///     Represents the association between a task and a tag
/// </summary>
/// <param name="TaskId">The ID of the task</param>
/// <param name="TagId">The ID of the tag</param>
/// <param name="CreatedAt">When this association was created</param>
public record TaskTagDto(
    string TaskId,
    string TagId,
    DateTime CreatedAt
);