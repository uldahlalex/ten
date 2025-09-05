using System.Net.Http.Headers;
using api;
using api.Etc;
using api.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using tests;

public static class ApiTestSetupUtilities
{
    public static WebApplicationBuilder MakeWebAppBuilderForTesting()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = "Development";

        // Get the API project directory for configuration files
        var currentDir = Directory.GetCurrentDirectory(); // /server/tests/
        var serverDir = Directory.GetParent(currentDir)?.FullName; // /server/
        var apiDir = Path.Combine(serverDir ?? "", "api");
        
        // Check if the API directory exists and adjust path if needed
        if (!Directory.Exists(apiDir))
        {
            // If we're in a different directory structure, try to find the API project
            var testDir = currentDir;
            while (testDir != null && !Directory.Exists(Path.Combine(testDir, "api")))
            {
                testDir = Directory.GetParent(testDir)?.FullName;
            }
            
            if (testDir != null)
            {
                apiDir = Path.Combine(testDir, "api");
            }
        }
        
        if (Directory.Exists(apiDir))
        {
            builder.Configuration
                .SetBasePath(apiDir)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
        }

        return builder;
    }

    public static WebApplicationBuilder AddProgramcsServices(this WebApplicationBuilder builder)
    {
        Program.ConfigureServices(builder.Services);
        return builder;
    }

    
    public static WebApplicationBuilder ModifyServicesForTesting(
        this WebApplicationBuilder builder,
        bool useTestContainer = true)
    {
        // Configure logging to show actual exceptions instead of hiding them
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        // Get the current AppOptions to copy other settings
        var currentAppOptions = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;

        // Create a unique test database for each test to avoid concurrency issues
        var testId = Guid.NewGuid().ToString("N")[..8];
        //var pgctx = new PgCtxSetup<MyDbContext>();
        
        // Remove existing DbContext registration
        var dbContextDescriptor = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions<MyDbContext>));
        if (dbContextDescriptor != null)
            builder.Services.Remove(dbContextDescriptor);
            
        var dbContextServiceDescriptor = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(MyDbContext));
        if (dbContextServiceDescriptor != null)
            builder.Services.Remove(dbContextServiceDescriptor);

        // Remove existing AppOptions registrations
        builder.Services.RemoveAll<IOptions<AppOptions>>();
        builder.Services.RemoveAll<IOptionsMonitor<AppOptions>>();
        builder.Services.RemoveAll<IOptionsSnapshot<AppOptions>>();
        
        // Create new AppOptions with test database connection string
       // var testConnectionString = pgctx._postgres.GetConnectionString() + $";SearchPath=test_{testId}";
        var testAppOptions = new AppOptions
        {
           // DbConnectionString = testConnectionString,
            JwtSecret = currentAppOptions.JwtSecret,
            RunsOn = currentAppOptions.RunsOn
        };
        
        // Register new AppOptions
        builder.Services.Configure<AppOptions>(options =>
        {
            //use testAppOption
            options.DbConnectionString = testAppOptions.DbConnectionString;
            options.JwtSecret = testAppOptions.JwtSecret;
            options.RunsOn = testAppOptions.RunsOn;
        });

        // Register new DbContext with test connection string
        builder.Services.AddDbContext<MyDbContext>(options =>
        {
            //options.UseNpgsql(testConnectionString);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Replace TimeProvider with test version
        var timeProviderDescriptor = builder.Services.SingleOrDefault(d => d.ServiceType == typeof(TimeProvider));
        if (timeProviderDescriptor != null)
            builder.Services.Remove(timeProviderDescriptor);

        builder.Services.AddSingleton<TimeProvider>(new FakeTimeProvider(StaticConstants.BaseDate));

        // Replace port allocation service for testing
        builder.Services.RemoveAll<IWebHostPortAllocationService>();
        builder.Services.AddSingleton<IWebHostPortAllocationService, TestPortAllocationService>();
        
        return builder;
    }

    public static WebApplication BeforeProgramcsMiddleware(this WebApplication app)
    {
        return app;
    }

    public static WebApplication AddProgramcsMiddleware(this WebApplication app)
    {
        // app.ConfigureStaticFilesForTesting();
        Program.ConfigureApp(app);
        return app;
    }

    public static WebApplication AfterProgramcsMiddleware(this WebApplication app)
    {
        app.StartAsync();
        return app;
    }

    // // Extension method to configure static files for testing
    // public static WebApplication ConfigureStaticFilesForTesting(this WebApplication app)
    // {
    //     // Serve client dist files from /client/dist/
    //     // Navigate up from /server/Start.Tests/ to root, then to /client/dist/
    //     var currentDir = Directory.GetCurrentDirectory(); // /server/Start.Tests/
    //     var serverDir = Directory.GetParent(currentDir)?.FullName; // /server/
    //     var rootDir = Directory.GetParent(serverDir)?.FullName; // /
    //     var clientDistPath = Path.Combine(rootDir ?? "", "client", "dist");
    //
    //     if (Directory.Exists(clientDistPath))
    //     {
    //         // Serve SPA at a dedicated path to avoid conflicts with API routes
    //         app.UseStaticFiles(new StaticFileOptions
    //         {
    //             FileProvider = new PhysicalFileProvider(clientDistPath),
    //             RequestPath = "/app"
    //         });
    //
    //         // SPA fallback - serve index.html for any unmatched routes under /app
    //         app.MapFallbackToFile("/app/{*path:nonfile}", "/index.html", new StaticFileOptions
    //         {
    //             FileProvider = new PhysicalFileProvider(clientDistPath)
    //         });
    //
    //         // Optional: Redirect root to SPA
    //         app.MapGet("/", () => Results.Redirect("/app"));
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Warning: Client dist directory not found at: {clientDistPath}");
    //     }
    //
    //     return app;
    // }

    public static HttpClient CreateHttpClientWithDefaultTestJwt()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(
         Jwt   );
        return client;
    }

    /// <summary>
    /// Decoded it becomes {"id": "user-1" }
    /// </summary>
    public static string Jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6InVzZXItMSJ9.LUnCy-TvtvyRhFyyg2qFFwhGMLYAFFFqrKEcBLFAf1Q";
}