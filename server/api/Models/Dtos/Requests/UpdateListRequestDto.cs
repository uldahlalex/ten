using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Basically just a change name of list since there are so few properties to lists
/// </summary>
public class UpdateListRequestDto
{
    public UpdateListRequestDto(string listId, string newName)
    {
        ListId = listId;
        NewName = newName;
    }

    [Required] public string ListId { get; set; } = null!;

    [Required] public string NewName { get; set; } = null!;
}