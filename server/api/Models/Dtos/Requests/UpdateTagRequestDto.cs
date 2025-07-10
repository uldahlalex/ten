using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of tag since there are so few properties to tags
/// </summary>
/// <param name="TagId">The unique identifier of the tag to update</param>
/// <param name="NewName">The new name for the tag</param>
public record UpdateTagRequestDto(
    string TagId,
    string NewName
);