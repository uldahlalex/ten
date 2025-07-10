namespace api.Models.Dtos.Responses;

/// <summary>
///     Represents a task list that can contain tasks
/// </summary>
/// <param name="ListId">Unique identifier for the list</param>
/// <param name="UserId">ID of the user who owns this list</param>
/// <param name="Name">The name of the list</param>
/// <param name="CreatedAt">When the list was created</param>
public record TasklistDto(
    string ListId,
    string UserId,
    string Name,
    DateTime CreatedAt
);