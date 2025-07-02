using System.Net.Http.Headers;
using api;
using api.Etc;
using api.Models;
using Infrastructure.Postgres.Scaffolding;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using PgCtx;
using tests;

public static class ApiTestSetupUtilities
{
    public static WebApplicationBuilder MakeWebAppBuilderForTesting()
    {
         var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = "Development";

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true);

        return builder;
    }

    public static WebApplicationBuilder AddProgramcsServices(this WebApplicationBuilder builder)
    {
        Program.ConfigureServices(builder);
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
        
        var appOptions = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;
        if (useTestContainer || appOptions.RunsOn == "GitHub")
        {
            // Create a unique test database for each test to avoid concurrency issues
            var testId = Guid.NewGuid().ToString("N")[..8];
            var pgctx = new PgCtxSetup<MyDbContext>();
            var startingDbCtx = builder.Services.FirstOrDefault(t => t.ServiceType == typeof(MyDbContext));
            builder.Services.Remove(startingDbCtx);
            builder.Services.AddDbContext<MyDbContext>(opt =>
            {
                var connectionString = pgctx._postgres.GetConnectionString() + $";SearchPath=test_{testId}";
                opt.UseNpgsql(connectionString);
                opt.EnableSensitiveDataLogging();
                opt.EnableDetailedErrors();
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }

        var timeProviderDescriptor = builder.Services.SingleOrDefault(d => d.ServiceType == typeof(TimeProvider));
        if (timeProviderDescriptor != null)
            builder.Services.Remove(timeProviderDescriptor);

        builder.Services.AddSingleton<TimeProvider>(new FakeTimeProvider(StaticConstants.BaseDate));

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
        // Configure static files BEFORE API middleware
        app.ConfigureStaticFilesForTesting();
        Program.ConfigureApp(app);
        return app;
    }

    public static WebApplication AfterProgramcsMiddleware(this WebApplication app)
    {
        app.StartAsync();
        return app;
    }

    // Extension method to configure static files for testing
    public static WebApplication ConfigureStaticFilesForTesting(this WebApplication app)
    {
        // Serve client dist files from /client/dist/
        // Navigate up from /server/Start.Tests/ to root, then to /client/dist/
        var currentDir = Directory.GetCurrentDirectory(); // /server/Start.Tests/
        var serverDir = Directory.GetParent(currentDir)?.FullName; // /server/
        var rootDir = Directory.GetParent(serverDir)?.FullName; // /
        var clientDistPath = Path.Combine(rootDir ?? "", "client", "dist");

        if (Directory.Exists(clientDistPath))
        {
            // Serve SPA at a dedicated path to avoid conflicts with API routes
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(clientDistPath),
                RequestPath = "/app"
            });

            // SPA fallback - serve index.html for any unmatched routes under /app
            app.MapFallbackToFile("/app/{*path:nonfile}", "/index.html", new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(clientDistPath)
            });

            // Optional: Redirect root to SPA
            app.MapGet("/", () => Results.Redirect("/app"));
        }
        else
        {
            Console.WriteLine($"Warning: Client dist directory not found at: {clientDistPath}");
        }

        return app;
    }

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