using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class Tag
{
    public string TagId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();

    public virtual User User { get; set; } = null!;
}
