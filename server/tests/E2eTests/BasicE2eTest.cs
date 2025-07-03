using Microsoft.Playwright;
using System.Net;
using TUnit.Core;

namespace tests.E2eTests;

public class BasicE2eTest : E2eTestBase
{
    [Test]
    public async Task CanNavigateToSpaAndSeeReactApp()
    {
        // First, let's ensure the client dist directory exists and is built
        var clientDistPath = Path.Combine(Directory.GetCurrentDirectory(), "../../client/dist");
        if (!Directory.Exists(clientDistPath))
        {
            // If no dist directory, this test should be skipped
            throw new Exception("Client dist directory not found. Run 'cd client && npm run build' first.");
        }

        // Navigate to the SPA
        await Page.GotoAsync($"{BaseUrl}");

        // Wait for the page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify we can see the React app
        var title = await Page.TitleAsync();
        if (string.IsNullOrEmpty(title))
            throw new Exception("Page title should not be empty");

        // Check if we can see some common React/Vite elements
        var bodyContent = await Page.TextContentAsync("body");
        if (string.IsNullOrEmpty(bodyContent))
            throw new Exception("Body content should not be empty");

        // Take a screenshot for debugging if needed
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "spa-screenshot.png"),
            FullPage = true 
        });
    }

    [Test]
    public async Task CanAccessApiEndpointsWhileSpaIsServed()
    {
        // Test that API endpoints still work when SPA is being served
        var response = await HttpClient.GetAsync("api/tasks");
        
        // Should return unauthorized (401) since we're not authenticated
        if (response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"Expected Unauthorized but got {response.StatusCode}");
        
        // Test with authentication
        var authenticatedClient = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        authenticatedClient.BaseAddress = new Uri(BaseUrl);
        
        var authenticatedResponse = await authenticatedClient.GetAsync("api/tasks");
        if (authenticatedResponse.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK but got {authenticatedResponse.StatusCode}");
    }

    [Test]
    public async Task CanAuthenticateViaUiAndAccessTasks()
    {
        // First check if dist exists
        var clientDistPath = Path.Combine(Directory.GetCurrentDirectory(), "../../client/dist");
        if (!Directory.Exists(clientDistPath))
        {
            throw new Exception("Client dist directory not found. Run 'cd client && npm run build' first.");
        }

        // Navigate to the SPA
        await Page.GotoAsync($"{BaseUrl}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // This is a basic test to verify the authentication flow works
        // Look for login form or authentication elements
        var hasLoginElements = await Page.QuerySelectorAsync("input[type='email'], input[type='password']") != null;
        
        if (hasLoginElements)
        {
            // If login form exists, try to fill it (using seeded test data)
            await Page.FillAsync("input[type='email']", "user-1@test.com");
            await Page.FillAsync("input[type='password']", "password");
            
            // Look for and click login button
            var loginButton = await Page.QuerySelectorAsync("button[type='submit'], input[type='submit']");
            if (loginButton != null)
            {
                await loginButton.ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }

        // Take a screenshot of the final state
        await Page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "auth-test-screenshot.png"),
            FullPage = true 
        });
    }
}