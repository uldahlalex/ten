using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class TagDto
{
    public string TagId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public TagDto FromEntity(Tag tag)
    {
        var dto = new TagDto
        {
            TagId = tag.TagId,
            Name = tag.Name,
            UserId = tag.UserId,
            CreatedAt = tag.CreatedAt,
            
        };
        return dto;
    }
}
