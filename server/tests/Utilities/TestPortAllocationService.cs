using System.Net;
using System.Net.Sockets;
using api.Etc;

namespace tests;

public class TestPortAllocationService : IWebHostPortAllocationService
{
    private static readonly object _lock = new();
    private static readonly HashSet<int> _usedPorts = new();
    private readonly int _port;

    public TestPortAllocationService()
    {
        lock (_lock)
        {
            _port = FindAvailablePort(8081, 9000);
        }
    }

    public string GetBaseUrl()
    {
        return $"http://localhost:{_port}";
    }

    private static int FindAvailablePort(int start, int end)
    {
        var ipAddress = IPAddress.Parse("127.0.0.1");

        for (var port = start; port <= end; port++)
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
            }
        }

        throw new Exception($"No available ports found between {start} and {end}");
    }
}