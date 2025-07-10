using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of list since there are so few properties to lists
/// </summary>
/// <param name="ListId">The unique identifier of the list to update</param>
/// <param name="NewName">The new name for the list</param>
public record UpdateListRequestDto(
    string ListId,
    string NewName
);