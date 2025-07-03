using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.AspNetCore.Builder;
using TUnit.Core;

namespace tests.E2eTests;

public class E2eTestBase : IAsyncDisposable
{
    private HttpClient? _httpClient;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private WebApplication? _app;
    private string? _baseUrl;

    [Before(Test)]
    public async Task SetupAsync()
    {
        // Build the web application with test configuration
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting()
            .AddProgramcsServices()
            .ModifyServicesForTesting();

        _app = builder.Build();
        _app.BeforeProgramcsMiddleware()
            .AddProgramcsMiddleware()
            .AfterProgramcsMiddleware();

        // Get the base URL
        _baseUrl = _app.Urls.First() + "/";

        // Initialize Playwright - check if we should connect to remote server
        var wsEndpoint = Environment.GetEnvironmentVariable("PW_TEST_CONNECT_WS_ENDPOINT");
        if (!string.IsNullOrEmpty(wsEndpoint))
        {
            // Connect to remote Playwright server running in Docker
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectAsync(wsEndpoint);
        }
        else
        {
            // Use local Playwright installation
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
        }
        
        // Create a new page
        _page = await _browser.NewPageAsync();
        
        // Create HTTP client for API calls
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    [After(Test)]
    public async Task TeardownAsync()
    {
        await DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
            _page = null;
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;

        _httpClient?.Dispose();
        _httpClient = null;

        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
            _app = null;
        }
    }

    protected IPage Page => _page ?? throw new InvalidOperationException("Test not properly initialized");
    protected HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Test not properly initialized");
    protected string BaseUrl => _baseUrl ?? throw new InvalidOperationException("Test not properly initialized");
    protected WebApplication App => _app ?? throw new InvalidOperationException("Test not properly initialized");
}