namespace api.Models.Dtos.Requests;

/// <summary>
///     If no value is passed to each property it defaults to not filter by the property. No value is required.
///     Deliberately using ? C# operator for properties such that if no default values are assigned, service method assigns
///     them manually
/// </summary>
public record GetTasksFilterAndOrderParameters(
    bool? IsCompleted = null,
    DateTime? EarliestDueDate = null,
    DateTime? LatestDueDate = null,
    int? MinPriority = null,
    int? MaxPriority = null,
    string? SearchTerm = null,
    List<string>? TagIds = null,
    List<string>? ListIds = null,
    string? OrderBy = null,
    bool? IsDescending = null
);