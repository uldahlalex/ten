using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class CreateListRequestDto
{
    [MinLength(1)] [Required] public string ListName { get; set; }
}