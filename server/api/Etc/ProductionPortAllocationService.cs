namespace api.Etc;

public class ProductionPortAllocationService : IWebHostPortAllocationService
{
    private const int Port = 8080;

    public string GetBaseUrl()
    {
        return $"http://0.0.0.0:{Port}";
    }
}