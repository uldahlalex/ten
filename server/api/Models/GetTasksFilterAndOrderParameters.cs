public class GetTasksFilterAndOrderParameters
{
    public bool? IsCompleted { get; set; }
    public DateTime? DueDateStart { get; set; } = DateTime.MinValue;
    public DateTime? DueDateEnd { get; set; } = DateTime.MaxValue;
    public int? MinPriority { get; set; }
    public int? MaxPriority { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? TagIds { get; set; }
    public List<string>? ListIds { get; set; }
    public string? OrderBy { get; set; }
    public bool IsDescending { get; set; }
}