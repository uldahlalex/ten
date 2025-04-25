using System;
using System.Collections.Generic;

namespace efscaffold.Entities;

public partial class TaskTagDto
{
    public string TaskId { get; set; } = null!;

    public string TagId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    
    public TaskTagDto FromEntity(TaskTag taskTag)
    {
        var dto = new TaskTagDto
        {
            TaskId = taskTag.TaskId,
            TagId = taskTag.TagId,
            CreatedAt = taskTag.CreatedAt,
        };
        return dto;
    }
}
