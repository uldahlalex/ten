// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

/// <summary>
/// Since "task" name is taken i use Tickticktask 
/// </summary>
public partial class Tickticktask
{
    public Tickticktask( string listId, string title, string description, DateTime? dueDate, int priority, bool completed, DateTime? completedAt, DateTime? createdAt = null, string? taskId = null)
    {
        TaskId = taskId ?? Guid.NewGuid().ToString();
        ListId = listId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        Completed = completed;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        CompletedAt = completedAt;
    }
}