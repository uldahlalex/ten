using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class UpdateListRequestDto
{
    [Required] public string ListId { get; set; }= null!;

    [Required] public string NewName { get; set; }= null!;
}