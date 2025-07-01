namespace api.Etc;

public static class StaticConstants
{
    public static string TickTickClone { get; set; } = nameof(TickTickClone);
    public static readonly DateTime BaseDate = new(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

}