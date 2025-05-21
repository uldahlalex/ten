using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
/// List is always created for the user sending the request
/// </summary>
public class CreateListRequestDto
{
    public CreateListRequestDto(string listName)
    {
        ListName = listName;
    }

    [MinLength(1)] [Required] public string ListName { get; set; }= null!;
}