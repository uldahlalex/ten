using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Tag is always created for the user sending the request
/// </summary>
public class CreateTagRequestDto
{
    public CreateTagRequestDto(string tagName)
    {
        TagName = tagName;
    }

    [Required] public string TagName { get; set; } = null!;
}