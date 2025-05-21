namespace api.Models.Dtos.Requests;

/// <summary>
///     If no value is passed to each property it defaults to not filter by the property. No value is required.
///     Deliberately using ? C# operator for properties such that if no default values are assigned, service method assigns
///     them manually
///     No constructor due to the above.
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