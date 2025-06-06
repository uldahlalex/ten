public static class SystemTime
{
    private static TimeProvider _timeProvider = TimeProvider.System;
    
    public static DateTime UtcNow => _timeProvider.GetUtcNow().DateTime;
    
    internal static void SetTimeProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    internal static void Reset()
    {
        _timeProvider = TimeProvider.System;
    }
}