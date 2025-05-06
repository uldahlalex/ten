using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos;

public class TasklistDto
{
    [Required] public string ListId { get; set; } = null!;

    [Required] public string UserId { get; set; } = null!;

    [Required] public string Name { get; set; } = null!;

    [Required] public DateTime CreatedAt { get; set; }


    //   [Required] public virtual ICollection<TickticktaskDto> Tickticktasks { get; set; } = new List<TickticktaskDto>(); //todo er denne i praktisk brug i client app og fetcher vi overhovedet med EF?
}