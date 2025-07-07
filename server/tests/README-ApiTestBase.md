# ApiTestBase Usage Guide

The `ApiTestBase` class provides a flexible foundation for API integration tests with hooks at different phases of the test setup process.

## Overview

`ApiTestBase` follows the same phases as `ApiTestSetupUtilities` but provides virtual hooks that you can override to customize behavior:

1. **Services Added** - After `Program.ConfigureServices`
2. **Services Modified** - After `ModifyServicesForTesting`
3. **Before Middleware** - After app is built, before middleware
4. **After Middleware** - After `Program.ConfigureApp`, before app starts
5. **Setup Complete** - After everything is ready for testing

## Basic Usage

```csharp
public class MyApiTests : ApiTestBase
{
    [Test]
    public async Task MyTest()
    {
        // Use _app, _client, _apiClient, _scopedServiceProvider
        var result = await _apiClient.SomeEndpointAsync();
        // Assert...
    }
}
```

## Customization Hooks

### 1. OnAfterServicesAdded(WebApplicationBuilder builder)

Called after `Program.ConfigureServices` but before test modifications. Use this to:
- Replace services with mocks before test setup
- Add custom services
- Modify logging configuration

```csharp
protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
{
    var mock = new Mock<IMyService>();
    var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IMyService));
    if (descriptor != null)
        builder.Services.Remove(descriptor);
    builder.Services.AddScoped<IMyService>(_ => mock.Object);
    
    return Task.CompletedTask;
}
```

### 2. OnAfterServicesModified(WebApplicationBuilder builder)

Called after test modifications but before building the app. Use this to:
- Make final service adjustments
- Override test configurations
- Add test-specific services

```csharp
protected override Task OnAfterServicesModified(WebApplicationBuilder builder)
{
    builder.Services.Configure<AppOptions>(options =>
    {
        options.SomeTestSetting = "CustomValue";
    });
    
    return Task.CompletedTask;
}
```

### 3. OnBeforeMiddleware(WebApplication app)

Called after the app is built but before standard middleware. Use this to:
- Add early middleware
- Configure request pipeline before standard setup

```csharp
protected override Task OnBeforeMiddleware(WebApplication app)
{
    app.UseMiddleware<MyCustomMiddleware>();
    return Task.CompletedTask;
}
```

### 4. OnAfterMiddleware(WebApplication app)

Called after standard middleware but before the app starts. Use this to:
- Add final middleware
- Modify routing
- Add custom endpoints

```csharp
protected override Task OnAfterMiddleware(WebApplication app)
{
    app.Use(async (context, next) =>
    {
        // Custom logic
        await next();
    });
    
    app.MapGet("/test-endpoint", () => "Test");
    
    return Task.CompletedTask;
}
```

### 5. OnSetupComplete()

Called when everything is ready for testing. Use this to:
- Perform final initialization
- Set up test data beyond the standard seeder
- Initialize test state

```csharp
protected override Task OnSetupComplete()
{
    // Initialize test-specific data
    return Task.CompletedTask;
}
```

### 6. Cleanup Hooks

```csharp
protected override Task OnBeforeCleanup()
{
    // Custom cleanup before resource disposal
    return Task.CompletedTask;
}

protected override Task OnAfterCleanup()
{
    // Final cleanup after all resources disposed
    return Task.CompletedTask;
}
```

## Common Patterns

### Mocking Services

```csharp
public class MockedServiceTest : ApiTestBase
{
    private Mock<IUserService> _mockUserService = null!;

    protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        _mockUserService = new Mock<IUserService>();
        
        // Remove and replace service
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IUserService));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<IUserService>(_ => _mockUserService.Object);
        
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestWithMock()
    {
        _mockUserService.Setup(x => x.GetUser(It.IsAny<string>()))
                       .ReturnsAsync(new User { Id = "test" });
        
        // Test logic...
    }
}
```

### Custom Configuration

```csharp
public class CustomConfigTest : ApiTestBase
{
    protected override Task OnAfterServicesModified(WebApplicationBuilder builder)
    {
        builder.Services.Configure<MyOptions>(options =>
        {
            options.TestMode = true;
            options.ApiKey = "test-key";
        });
        
        return Task.CompletedTask;
    }
}
```

### Adding Test Middleware

```csharp
public class MiddlewareTest : ApiTestBase
{
    protected override Task OnAfterMiddleware(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Test", "true");
            await next();
        });
        
        return Task.CompletedTask;
    }
}
```

### Complex Setup

```csharp
public class ComplexTest : ApiTestBase
{
    private Mock<IService1> _mock1 = null!;
    private Mock<IService2> _mock2 = null!;

    protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        // Replace multiple services
        _mock1 = new Mock<IService1>();
        _mock2 = new Mock<IService2>();
        
        ReplaceService<IService1>(builder, _mock1.Object);
        ReplaceService<IService2>(builder, _mock2.Object);
        
        return Task.CompletedTask;
    }

    protected override Task OnAfterServicesModified(WebApplicationBuilder builder)
    {
        // Add custom test configuration
        builder.Services.AddScoped<ITestHelper, TestHelper>();
        return Task.CompletedTask;
    }

    protected override Task OnAfterMiddleware(WebApplication app)
    {
        // Add response transformation
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 404)
            {
                context.Response.Headers.Add("X-Not-Found", "true");
            }
        });
        
        return Task.CompletedTask;
    }

    private static void ReplaceService<T>(WebApplicationBuilder builder, T implementation) 
        where T : class
    {
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<T>(_ => implementation);
    }
}
```

## Migration from Manual Setup

### Before (Manual Setup)
```csharp
public class MyTest
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private IApiClient _apiClient = null!;

    [Before(Test)]
    public Task Setup()
    {
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting();
        builder.AddProgramcsServices();
        
        // Custom service replacement
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IMyService));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<IMyService, MockMyService>();
        
        builder.ModifyServicesForTesting();
        _app = builder.Build();
        _app.BeforeProgramcsMiddleware();
        _app.AddProgramcsMiddleware();
        _app.AfterProgramcsMiddleware();

        var baseUrl = _app.Urls.First() + "/";
        _client = new HttpClient();
        _apiClient = new ApiClient(baseUrl, _client);
        
        return Task.CompletedTask;
    }
}
```

### After (Using ApiTestBase)
```csharp
public class MyTest : ApiTestBase
{
    protected override Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IMyService));
        if (descriptor != null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<IMyService, MockMyService>();
        
        return Task.CompletedTask;
    }
    
    // Tests use _app, _client, _apiClient automatically
}
```

## Available Properties

- `_app`: The `WebApplication` instance
- `_client`: The `HttpClient` for making requests
- `_apiClient`: The generated API client
- `_scopedServiceProvider`: Service provider for accessing services

All properties are available in test methods and hook methods after `Setup()` is called.