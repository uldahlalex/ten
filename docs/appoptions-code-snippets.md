# AppOptions Configuration - Code Snippets

## 1. AppOptions Model (`api/Models/AppOptions.cs`)

```csharp
public sealed class AppOptions
{
    [Required] public string JwtSecret { get; set; } = string.Empty!;
    [Required] public string DbConnectionString { get; set; } = string.Empty!;
    public string RunsOn { get; set; } = string.Empty!;
}
```

## 2. Configuration Sources

### appsettings.json (Production)
```json
{
  "AppOptions": {
    "JwtSecret": "verylongstring",
    "DbConnectionString": "Server=someURL"
  }
}
```

### appsettings.Development.json (Development)
```json
{
  "AppOptions": {
    "JwtSecret": "verylongstring",
    "DbConnectionString": "Server=someURL"
  }
}
```

### Environment Variables Override
```bash
# Any AppOptions property can be overridden via environment variables
AppOptions__JwtSecret="your-jwt-secret-here"
AppOptions__DbConnectionString="your-db-connection-string"
AppOptions__RunsOn="Production"
```

## 3. Registration & Validation (`api/Etc/AppOptionsExtensions.cs`)

```csharp
public static AppOptions AddAppOptions(this IServiceCollection services, IConfiguration configuration)
{
    var appOptions = new AppOptions();
    configuration.GetSection(nameof(AppOptions)).Bind(appOptions);

    services.Configure<AppOptions>(configuration.GetSection(nameof(AppOptions)));

    ICollection<ValidationResult> results = new List<ValidationResult>();
    var validated = Validator.TryValidateObject(appOptions, new ValidationContext(appOptions), results, true);
    if (!validated)
        throw new Exception(
            $"hey buddy, alex here. You're probably missing an environment variable / appsettings.json stuff / repo secret on github. Here's the technical error: " +
            $"{string.Join(", ", results.Select(r => r.ErrorMessage))}");

    return appOptions;
}
```

## 4. Service Registration (`api/Program.cs`)

```csharp
public static void ConfigureServices(WebApplicationBuilder builder)
{
    // ... other services ...
    
    var appOptions = builder.Services.AddAppOptions(builder.Configuration);
    
    // ... rest of service registration ...
}
```

## 5. Consumption in Services

### AuthenticationService
```csharp
public class AuthenticationService(
    IUserDataService userDataService, 
    IOptionsMonitor<AppOptions> optionsMonitor,  // <-- Injected here
    ICryptographyService cryptographyService, 
    IJwtService jwtService, 
    ITotpService totpService) : IAuthenticationService
{
    // Uses optionsMonitor.CurrentValue to access configuration
}
```

## Configuration Hierarchy (Precedence)

1. **Environment Variables** (highest priority)
2. **appsettings.{Environment}.json** (e.g., appsettings.Development.json)
3. **appsettings.json** (lowest priority)

## Key Features

- **Validation**: Uses DataAnnotations (`[Required]`) with custom error messages
- **Type Safety**: Strongly typed configuration via `AppOptions` class
- **Hot Reload**: `IOptionsMonitor<T>` supports configuration changes at runtime
- **Dependency Injection**: Registered with DI container for easy consumption
- **Environment Overrides**: Supports different configurations per environment