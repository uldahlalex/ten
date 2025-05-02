using System.Net;
using System.Net.Sockets;

public interface IWebHostPortAllocationService
{
    string GetBaseUrl();
}

public class ProductionPortAllocationService : IWebHostPortAllocationService
{
    private const int Port = 8080;
    public string GetBaseUrl() => $"http://0.0.0.0:{Port}";
}

public class TestPortAllocationService : IWebHostPortAllocationService
{
    private readonly int _port;
    private static readonly object _lock = new object();
    private static readonly HashSet<int> _usedPorts = new HashSet<int>();

    public TestPortAllocationService()
    {
        lock (_lock)
        {
            _port = FindAvailablePort(8081, 9000);
        }
    }

    public string GetBaseUrl() => $"http://localhost:{_port}";  // Using localhost instead of 127.0.0.1

    private static int FindAvailablePort(int start, int end)
    {
        var ipAddress = IPAddress.Parse("127.0.0.1");
        
        for (int port = start; port <= end; port++)
        {
            if (_usedPorts.Contains(port))
                continue;

            try
            {
                var listener = new TcpListener(ipAddress, port);
                listener.Start();
                listener.Stop();
                _usedPorts.Add(port);
                return port;
            }
            catch (SocketException)
            {
                continue;
            }
        }
        throw new Exception($"No available ports found between {start} and {end}");
    }
}