namespace api.Models.Dtos.Requests;

/// <summary>
///     Tag is always created for the user sending the request
/// </summary>
/// <param name="TagName">The name of the new tag to create</param>
public record CreateTagRequestDto(
    string TagName
);