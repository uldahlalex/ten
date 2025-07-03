using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.AspNetCore.Builder;
using TUnit.Core;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Runtime.InteropServices;

namespace tests.E2eTests;

public class DockerManagedE2eTestBase : IAsyncDisposable
{
    private HttpClient? _httpClient;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private WebApplication? _app;
    private string? _baseUrl;
    private DockerClient? _dockerClient;
    private string? _containerId;
    private static readonly SemaphoreSlim _dockerSemaphore = new(1, 1);
    private static readonly Dictionary<string, int> _activeContainers = new();
    private const int PLAYWRIGHT_PORT = 3000;

    [Before(Test)]
    public async Task SetupAsync()
    {
        await _dockerSemaphore.WaitAsync();
        try
        {
            // Start Playwright Docker container
            await StartPlaywrightDockerContainerAsync();
            
            // Wait for Playwright server to be ready
            await WaitForPlaywrightServerAsync();
        }
        finally
        {
            _dockerSemaphore.Release();
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
        var wsEndpoint = $"ws://127.0.0.1:{PLAYWRIGHT_PORT}/";
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.ConnectAsync(wsEndpoint);
        
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

        // Stop Docker container if this was the last test using it
        await _dockerSemaphore.WaitAsync();
        try
        {
            await StopPlaywrightDockerContainerAsync();
        }
        finally
        {
            _dockerSemaphore.Release();
        }
    }

    private async Task StartPlaywrightDockerContainerAsync()
    {
        var testId = Environment.CurrentManagedThreadId.ToString();
        
        // Check if we already have a container running for this test session
        if (_activeContainers.ContainsKey(testId))
        {
            return; // Container already running
        }

        _dockerClient = new DockerClientConfiguration().CreateClient();

        var containerName = $"playwright-e2e-{testId}";

        // Check if container already exists
        var existingContainers = await _dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters { All = true });
        
        var existingContainer = existingContainers.FirstOrDefault(c => 
            c.Names.Any(name => name.Contains(containerName)));

        if (existingContainer != null)
        {
            // Container exists, start it if not running
            if (existingContainer.State != "running")
            {
                await _dockerClient.Containers.StartContainerAsync(existingContainer.ID, 
                    new ContainerStartParameters());
            }
            _containerId = existingContainer.ID;
        }
        else
        {
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
            catch (DockerApiException ex) when (ex.Message.Contains("pull access denied"))
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

        _activeContainers[testId] = 1;
    }

    private async Task StopPlaywrightDockerContainerAsync()
    {
        if (_dockerClient == null || _containerId == null) return;

        var testId = Environment.CurrentManagedThreadId.ToString();
        
        if (_activeContainers.ContainsKey(testId))
        {
            _activeContainers.Remove(testId);
        }

        // Only stop if no other tests are using the container
        if (_activeContainers.Count == 0)
        {
            try
            {
                await _dockerClient.Containers.StopContainerAsync(_containerId, 
                    new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
            }
            catch (DockerContainerNotFoundException)
            {
                // Container already stopped/removed
            }
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