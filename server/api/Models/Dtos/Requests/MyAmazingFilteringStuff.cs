namespace api.Models.Dtos.Requests;

/// <summary>
///     If no value is passed to each property it defaults to not filter by the property. No value is required.
///     Deliberately using ? C# operator for properties such that if no default values are assigned, service method assigns
///     them manually
/// </summary>
/// <param name="IsCompleted">Filter by completion status (optional)</param>
/// <param name="EarliestDueDate">Filter by earliest due date (optional)</param>
/// <param name="LatestDueDate">Filter by latest due date (optional)</param>
/// <param name="MinPriority">Filter by minimum priority level (optional)</param>
/// <param name="MaxPriority">Filter by maximum priority level (optional)</param>
/// <param name="SearchTerm">Search term to filter tasks (optional)</param>
/// <param name="TagIds">List of tag IDs to filter by (optional)</param>
/// <param name="ListIds">List of list IDs to filter by (optional)</param>
/// <param name="OrderBy">Field to order results by (optional)</param>
/// <param name="IsDescending">Whether to order in descending order (optional)</param>
public record MyAmazingFilteringStuff(
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