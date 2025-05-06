
using System.ComponentModel.DataAnnotations;

namespace api;

public class UpdateListRequestDto
{
    [Required]
    public string ListId { get; set; }
    
    [Required]
    public string NewName { get; set; }
}