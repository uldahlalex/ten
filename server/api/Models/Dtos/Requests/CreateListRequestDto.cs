using System.ComponentModel.DataAnnotations;

namespace api;

public class CreateListRequestDto
{
    [MinLength(1)]
    public string ListName { get; set; }
}