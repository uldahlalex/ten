using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Tag is always created for the user sending the request
/// </summary>
public record CreateTagRequestDto(
    [Required] string TagName
);