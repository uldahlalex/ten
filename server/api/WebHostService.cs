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

    public TestPortAllocationService()
    {
        _port = FindAvailablePort(8080, 8180);
    }

    public string GetBaseUrl() => $"http://127.0.0.1:{_port}";

    private static int FindAvailablePort(int start, int end)
    {
        for (int port = start; port <= end; port++)
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
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