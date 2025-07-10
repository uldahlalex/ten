using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of tag since there are so few properties to tags
/// </summary>
public record UpdateTagRequestDto(
    [Required] string TagId,
    [Required] string NewName
);