// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

/// <summary>
/// This is simply the junction table (many to many) between task and tag
/// </summary>
public partial class TaskTag
{

    /// <summary>
    /// Private parameterless constructor to be used by EF Core.
    /// </summary>
    private TaskTag()
    {
        
    }
    
    public TaskTag(DateTime createdAt,string taskId, string tagId)
    {
        CreatedAt = createdAt;
        TaskId = taskId;
        TagId = tagId;

    }
}