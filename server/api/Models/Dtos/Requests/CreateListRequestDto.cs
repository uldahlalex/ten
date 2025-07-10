namespace api.Models.Dtos.Requests;

/// <summary>
///     List is always created for the user sending the request
/// </summary>
/// <param name="ListName">The name of the new list to create</param>
public record CreateListRequestDto(
    string ListName
);