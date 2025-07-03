using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.AspNetCore.Builder;
using TUnit.Core;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace tests.E2eTests;

/// <summary>
/// E2E test base that can auto-start Playwright server or use existing one.
/// For IDE usage:
/// 1. Auto mode: Tests will start/stop Docker automatically (slower)
/// 2. Manual mode: Start Playwright server manually first (faster for development)
/// 
/// Manual mode setup:
/// - Run: docker run -d --name playwright-server --add-host=hostmachine:host-gateway -p 3000:3000 --rm --init --workdir /home/pwuser --user pwuser mcr.microsoft.com/playwright:v1.53.0-noble /bin/sh -c "npx -y playwright@1.53.0 run-server --port 3000 --host 0.0.0.0"
/// - Set environment variable: PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/
/// - Run tests from IDE
/// - Clean up: docker stop playwright-server
/// </summary>
public class IdeE2eTestBase : IAsyncDisposable
{
    private HttpClient? _httpClient;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private WebApplication? _app;
    private string? _baseUrl;
    private DockerClient? _dockerClient;
    private string? _containerId;
    private bool _managedDocker = false;
    private const int PLAYWRIGHT_PORT = 3000;

    [Before(Test)]
    public async Task SetupAsync()
    {
        // Check if manual Playwright server is already running
        var manualEndpoint = Environment.GetEnvironmentVariable("PW_TEST_CONNECT_WS_ENDPOINT");
        var useManualServer = !string.IsNullOrEmpty(manualEndpoint);

        if (!useManualServer)
        {
            // Auto-start Docker container
            await StartPlaywrightDockerContainerAsync();
            await WaitForPlaywrightServerAsync();
            _managedDocker = true;
        }

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

        // Connect to Playwright server
        var wsEndpoint = useManualServer ? manualEndpoint : $"ws://127.0.0.1:{PLAYWRIGHT_PORT}/";
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.ConnectAsync(wsEndpoint!);
        
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

        // Stop Docker container only if we started it
        if (_managedDocker)
        {
            await StopPlaywrightDockerContainerAsync();
        }
    }

    private async Task StartPlaywrightDockerContainerAsync()
    {
        _dockerClient = new DockerClientConfiguration().CreateClient();

        var containerName = $"playwright-ide-e2e-{Environment.ProcessId}";

        // Check if container already exists and is running
        var existingContainers = await _dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters { All = false });
        
        var runningContainer = existingContainers.FirstOrDefault(c => 
            c.Names.Any(name => name.Contains("playwright-ide-e2e")));

        if (runningContainer != null)
        {
            _containerId = runningContainer.ID;
            return; // Use existing container
        }

        // Create new container
        var createParams = new CreateContainerParameters
        {
            Image = "mcr.microsoft.com/playwright:v1.53.0-noble",
            Name = containerName,
            Cmd = new[] { "/bin/sh", "-c", $"npx -y playwright@1.53.0 run-server --port {PLAYWRIGHT_PORT} --host 0.0.0.0" },
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { $"{PLAYWRIGHT_PORT}/tcp", default }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {
                        $"{PLAYWRIGHT_PORT}/tcp",
                        new List<PortBinding>
                        {
                            new() { HostPort = PLAYWRIGHT_PORT.ToString() }
                        }
                    }
                },
                AutoRemove = true,
                ExtraHosts = new[] { "hostmachine:host-gateway" }
            },
            WorkingDir = "/home/pwuser",
            User = "pwuser"
        };

        try
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(createParams);
            _containerId = response.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, 
                new ContainerStartParameters());
        }
        catch (DockerApiException ex) when (ex.Message.Contains("pull access denied") || ex.Message.Contains("not found"))
        {
            // Try to pull the image first
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = "mcr.microsoft.com/playwright",
                    Tag = "v1.53.0-noble"
                },
                null,
                new Progress<JSONMessage>());

            var response = await _dockerClient.Containers.CreateContainerAsync(createParams);
            _containerId = response.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, 
                new ContainerStartParameters());
        }
    }

    private async Task StopPlaywrightDockerContainerAsync()
    {
        if (_dockerClient == null || _containerId == null) return;

        try
        {
            await _dockerClient.Containers.StopContainerAsync(_containerId, 
                new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
        }
        catch (DockerContainerNotFoundException)
        {
            // Container already stopped/removed
        }
        catch (Exception)
        {
            // Ignore cleanup errors
        }

        _dockerClient?.Dispose();
        _dockerClient = null;
        _containerId = null;
    }

    private async Task WaitForPlaywrightServerAsync()
    {
        using var httpClient = new HttpClient();
        var maxAttempts = 30;
        var delayMs = 1000;

        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                var response = await httpClient.GetAsync($"http://localhost:{PLAYWRIGHT_PORT}/");
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Server is responding (even with 404 is fine, means it's up)
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Server not ready yet
            }

            await Task.Delay(delayMs);
        }

        throw new InvalidOperationException("Playwright server failed to start within expected time");
    }

    protected IPage Page => _page ?? throw new InvalidOperationException("Test not properly initialized");
    protected HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Test not properly initialized");
    protected string BaseUrl => _baseUrl ?? throw new InvalidOperationException("Test not properly initialized");
    protected WebApplication App => _app ?? throw new InvalidOperationException("Test not properly initialized");
}