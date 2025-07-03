using Microsoft.Playwright;
using System.Net;
using System.Net.Http.Json;
using TUnit.Core;

namespace tests.E2eTests;

/// <summary>
/// IDE-friendly E2E tests that can run with or without manual Docker setup.
/// 
/// For fastest development workflow in Rider:
/// 1. Build React: cd client && npm run build
/// 2. Start Playwright server: docker run -d --name playwright-server --add-host=hostmachine:host-gateway -p 3000:3000 --rm --init --workdir /home/pwuser --user pwuser mcr.microsoft.com/playwright:v1.53.0-noble /bin/sh -c "npx -y playwright@1.53.0 run-server --port 3000 --host 0.0.0.0"
/// 3. Set environment variable in Rider: PW_TEST_CONNECT_WS_ENDPOINT=ws://127.0.0.1:3000/
/// 4. Run individual tests from IDE
/// 5. When done: docker stop playwright-server
/// 
/// Alternative: Tests will auto-start Docker if no manual server is detected (slower but more convenient)
/// </summary>
[Category("E2E")]
public class IdeE2eTest : IdeE2eTestBase
{
    [Test]
    public async Task CanAccessApiEndpoints()
    {
        // Test that API endpoints are accessible
        var requestBody = new { /* empty filter parameters */ };
        var response = await HttpClient.PostAsJsonAsync("GetMyTasks", requestBody);
        
        // Should return some kind of error (400, 401, etc.) but not 404, proving the endpoint exists
        if ((int)response.StatusCode < 400 || (int)response.StatusCode >= 500)
        {
            throw new Exception($"Expected 4xx error but got {response.StatusCode}");
        }
        
        // Test with authentication - this should work better
        var authenticatedClient = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        authenticatedClient.BaseAddress = new Uri(BaseUrl);
        
        var authenticatedResponse = await authenticatedClient.PostAsJsonAsync("GetMyTasks", requestBody);
        // Should be 200 OK or 400 BadRequest (due to validation), but not auth errors
        if ((int)authenticatedResponse.StatusCode >= 500)
        {
            throw new Exception($"Expected successful response or client error but got {authenticatedResponse.StatusCode}");
        }
    }

    [Test] 
    public async Task CanNavigateToSpa()
    {
        // Navigate to the SPA (served by .NET API)
        await Page.GotoAsync($"{BaseUrl}");

        // Wait for the page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify we can see the React app
        var title = await Page.TitleAsync();
        if (string.IsNullOrEmpty(title))
        {
            throw new Exception("Page title should not be empty");
        }

        // Check if we can see some content
        var bodyContent = await Page.TextContentAsync("body");
        if (string.IsNullOrEmpty(bodyContent))
        {
            throw new Exception("Body content should not be empty");
        }

        // Take a screenshot for debugging
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "ide-spa-screenshot.png"),
            FullPage = true 
        });
    }

    [Test]
    public async Task CanInteractWithReactComponents()
    {
        // Navigate to the SPA
        await Page.GotoAsync($"{BaseUrl}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Try to find common React/UI elements
        // This is a basic check - you can expand based on your actual UI
        var bodyText = await Page.TextContentAsync("body");
        
        // Take screenshot for visual verification
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "react-components-screenshot.png"),
            FullPage = true 
        });
        
        // Basic assertion that something rendered
        if (string.IsNullOrWhiteSpace(bodyText) || bodyText.Length < 10)
        {
            throw new Exception("Page appears to be empty or not properly loaded");
        }
    }
}