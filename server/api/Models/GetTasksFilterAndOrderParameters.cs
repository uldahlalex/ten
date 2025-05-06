namespace api.Models;

/// <summary>
///     deliberately using ? operator for properties such that no default values are assigned and i can always check for
///     nulls
/// </summary>
public class GetTasksFilterAndOrderParameters
{
    public bool? IsCompleted { get; set; }
    public DateTime? EarliestDueDate { get; set; }
    public DateTime? LatestDueDate { get; set; }
    public int? MinPriority { get; set; }
    public int? MaxPriority { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? TagIds { get; set; }
    public List<string>? ListIds { get; set; }
    public string? OrderBy { get; set; }
    public bool? IsDescending { get; set; }
}