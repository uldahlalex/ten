namespace api;

public class TaskQueryParams
{
    public bool? IsCompleted { get; set; }
    public DateTime? DueDateStart { get; set; }
    public DateTime? DueDateEnd { get; set; }
    public int? MinPriority { get; set; }
    public int? MaxPriority { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? TagIds { get; set; }
    public List<string>? ListIds { get; set; }
    public TaskOrderBy? OrderBy { get; set; } 
    public bool IsDescending { get; set; }
}