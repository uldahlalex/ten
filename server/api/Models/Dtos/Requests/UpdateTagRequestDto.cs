using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of tag since there are so few properties to tags
/// </summary>
public class UpdateTagRequestDto
{
    public UpdateTagRequestDto(string tagId, string newName)
    {
        TagId = tagId;
        NewName = newName;
    }

    [Required] public string TagId { get; set; } = null!;

    [Required] public string NewName { get; set; } = null!;
}