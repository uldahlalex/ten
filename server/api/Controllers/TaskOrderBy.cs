using efscaffold.Entities;

namespace api;

public class TaskOrderBy
{
    private TaskOrderBy(string value) { Value = value; }
    public string Value { get; }

    public static TaskOrderBy DueDate => new(nameof(Tickticktask.DueDate).ToLower());
    public static TaskOrderBy Priority => new(nameof(Tickticktask.Priority).ToLower());
    public static TaskOrderBy CreatedAt => new(nameof(Tickticktask.CreatedAt).ToLower());
    public static TaskOrderBy CompletedAt => new(nameof(Tickticktask.CompletedAt).ToLower());

    public static implicit operator string(TaskOrderBy orderBy) => orderBy.Value;
    
    public static bool TryParse(string? value, out TaskOrderBy? orderBy)
    {
        if (string.IsNullOrEmpty(value))
        {
            orderBy = null;
            return false;
        }

        value = value.ToLower();
        if (value == DueDate || value == Priority || 
            value == CreatedAt || value == CompletedAt)
        {
            orderBy = new TaskOrderBy(value);
            return true;
        }

        orderBy = null;
        return false;
    }
}