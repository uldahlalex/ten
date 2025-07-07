using api.Models.Dtos.Requests;
using api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using tests.Utilities;
using TUnit.Core;
using Generated;
using api.Etc;
using api.Models;
using Moq;

namespace tests.Examples;

/// <summary>
/// Example 1: Basic usage with no customization
/// </summary>
public class BasicApiTest : ApiTestBase
{
    [Test]
    public async Task Login_CanSuccessfully_Login()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        var dto = new AuthRequestDto("john@example.com", "password");
        var jwt = await ApiClient.Auth_LoginAsync(dto);
            
        var jwtService = ScopedServiceProvider.GetRequiredService<IJwtService>();
        var claims = jwtService.VerifyJwt(jwt.Jwt);
        
        if (claims.Id != ids.JohnId)
            throw new Exception($"Expected JWT to contain John's ID {ids.JohnId} but got {claims.Id}");
    }
}

/// <summary>
/// Example 2: Replacing a service with a mock after services are added
/// </summary>
public class MockedServiceTest : ApiTestBase
{
    private Mock<IUserDataService> _mockUserService = null!;

    protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        // Replace the user service with a mock before test modifications
        _mockUserService = new Mock<IUserDataService>();
        
        // Remove existing registration
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IUserDataService));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
            
        // Add mock
        builder.Services.AddScoped<IUserDataService>(_ => _mockUserService.Object);
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestWithMockedUserService()
    {
        // Setup mock behavior
        _mockUserService.Setup(x => x.UserExistsAsync(It.IsAny<string>()))
                       .ReturnsAsync(true);

        // Your test logic here...
        var userService = ScopedServiceProvider.GetRequiredService<IUserDataService>();
        var exists = await userService.UserExistsAsync("test-id");
        
        if (!exists)
            throw new Exception("Expected user to exist according to mock");
    }
}

/// <summary>
/// Example 3: Adding custom middleware
/// </summary>
public class CustomMiddlewareTest : ApiTestBase
{
    private bool _middlewareExecuted = false;

    protected override Task OnAfterMiddleware(WebApplication app)
    {
        // Add custom middleware after the standard middleware
        app.Use(async (context, next) =>
        {
            _middlewareExecuted = true;
            await next();
        });
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestCustomMiddleware()
    {
        // Make any request to trigger middleware
        try
        {
            await ApiClient.Auth_LoginAsync(new AuthRequestDto("test@test.com", "wrong"));
        }
        catch
        {
            // Ignore auth failure - we just want to trigger middleware
        }
        
        if (!_middlewareExecuted)
            throw new Exception("Custom middleware was not executed");
    }
}

/// <summary>
/// Example 4: Modifying configuration after test services are set up
/// </summary>
public class CustomConfigurationTest : ApiTestBase
{
    protected override Task OnAfterServicesModified(WebApplicationBuilder builder)
    {
        // Make final adjustments after all test modifications
        builder.Services.Configure<api.Models.AppOptions>(options =>
        {
            // Override some test configuration
            options.RunsOn = "CustomTestEnvironment";
        });
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestCustomConfiguration()
    {
        var appOptions = ScopedServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<api.Models.AppOptions>>();
        
        if (appOptions.Value.RunsOn != "CustomTestEnvironment")
            throw new Exception("Custom configuration was not applied");
    }
}

/// <summary>
/// Example 5: Early middleware configuration
/// </summary>
public class EarlyMiddlewareTest : ApiTestBase
{
    protected override Task OnBeforeMiddleware(WebApplication app)
    {
        // Add middleware before the standard Program.ConfigureApp middleware
        app.UseMiddleware<TestMiddleware>();
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestEarlyMiddleware()
    {
        // Test that early middleware is working
        var response = await Client.GetAsync("/api/tasks");
        // Assert middleware behavior...
    }
}

// Example custom middleware
public class TestMiddleware
{
    private readonly RequestDelegate _next;

    public TestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Add("X-Test-Middleware", "Executed");
        await _next(context);
    }
}

/// <summary>
/// Example 6: Complex setup with multiple customizations
/// </summary>
public class ComplexCustomizationTest : ApiTestBase
{
    private Mock<ITaskService> _mockTaskService = null!;

    protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        // Replace task service with mock
        _mockTaskService = new Mock<ITaskService>();
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(ITaskService));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<ITaskService>(_ => _mockTaskService.Object);
        
        return Task.CompletedTask;
    }

    protected override Task OnAfterServicesModified(WebApplicationBuilder builder)
    {
        // Add some custom test services
        builder.Services.AddScoped<ICustomTestService, CustomTestService>();
        return Task.CompletedTask;
    }

    protected override Task OnAfterMiddleware(WebApplication app)
    {
        // Add response transformation middleware
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.ContentType?.Contains("application/json") == true)
            {
                context.Response.Headers.Add("X-Json-Response", "true");
            }
        });
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestComplexSetup()
    {
        // Setup mock behavior (simplified for example)
        // Note: ITaskService methods would need to be mocked based on actual interface

        // Test the custom service
        var customService = ScopedServiceProvider.GetRequiredService<ICustomTestService>();
        var result = customService.DoSomething();
        
        if (result != "Test Result")
            throw new Exception("Custom service not working");
    }
}

// Example custom service
public interface ICustomTestService
{
    string DoSomething();
}

public class CustomTestService : ICustomTestService
{
    public string DoSomething() => "Test Result";
}