using Microsoft.Extensions.Logging;

public class TestLogger : ILogger
{
    private readonly List<string> _logMessages = new();
    public IReadOnlyList<string> LogMessages => _logMessages;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logMessages.Add(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}

