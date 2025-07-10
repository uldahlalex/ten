namespace api.Models.Dtos.Responses;

/// <summary>
///     Represents a tag that can be associated with tasks
/// </summary>
/// <param name="TagId">Unique identifier for the tag</param>
/// <param name="Name">The name of the tag</param>
/// <param name="UserId">ID of the user who owns this tag</param>
/// <param name="CreatedAt">When the tag was created</param>
public record TagDto(
    string TagId,
    string Name,
    string UserId,
    DateTime CreatedAt
);