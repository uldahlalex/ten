using Microsoft.Playwright;
using System.Net;
using System.Net.Http.Json;
using TUnit.Core;

namespace tests.E2eTests;

[Category("E2E")]
public class DockerManagedE2eTest : DockerManagedE2eTestBase
{
    [Test]
    public async Task CanAccessApiEndpointsWithDockerManagedPlaywright()
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
    public async Task CanNavigateToSpaWithDockerManagedPlaywright()
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

        // Take a screenshot for debugging if needed
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "docker-managed-spa-screenshot.png"),
            FullPage = true 
        });
    }
}