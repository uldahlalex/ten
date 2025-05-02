using System.Net;
using System.Net.Sockets;

public interface IWebHostPortAllocationService
{
    Task ConfigureUrlsAsync(WebApplication app);
}

public class ProductionPortAllocationService : IWebHostPortAllocationService
{
    public Task ConfigureUrlsAsync(WebApplication app)
    {
        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{Program.DefaultPort}");
        return Task.CompletedTask;
    }
}


    public class TestPortAllocationService : IWebHostPortAllocationService
    {
        public async Task ConfigureUrlsAsync(WebApplication app)
        {
            var port = Program.DefaultPort;
            bool portFound = false;

            while (!portFound && port < Program.DefaultPort + 100)
            {
                try
                {
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                        portFound = true;
                        Program.DefaultPort = port;
                    }
                }
                catch (SocketException)
                {
                    port++;
                }
            }

            if (!portFound)
            {
                throw new Exception($"Could not find an available port between {Program.DefaultPort} and {Program.DefaultPort + 100}");
            }

            app.Urls.Clear();
            app.Urls.Add($"http://127.0.0.1:{port}");
        }
    }
