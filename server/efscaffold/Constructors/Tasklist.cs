// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

/// <summary>
/// Is simply called tasklist because "List" is already taken in C#. It is just the list that tasks can belong to in the task manager
/// </summary>
public partial class Tasklist
{
    public Tasklist(string name, string userId, string? id = null, DateTime? createdAt = null)
    {
        ListId = id ?? Guid.NewGuid().ToString();
        UserId = userId;
        Name = name;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        
    }
}