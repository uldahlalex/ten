using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of list since there are so few properties to lists
/// </summary>
public record UpdateListRequestDto(
    [Required] string ListId,
    [Required] string NewName
);