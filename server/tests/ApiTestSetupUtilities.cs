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
using Microsoft.Extensions.Options;
using PgCtx;

namespace tests;

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
        var appOptions = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;
        if (useTestContainer || appOptions.RunsOn == "GitHub")
        {
            var pgctx = new PgCtxSetup<MyDbContext>();
            var startingDbCtx = builder.Services.FirstOrDefault(t => t.ServiceType == typeof(MyDbContext));
            builder.Services.Remove(startingDbCtx);
            builder.Services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(pgctx._postgres.GetConnectionString());
                Console.WriteLine(pgctx._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging();
                opt.LogTo(_ => { });
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }

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
        Program.ConfigureApp(app);
        return app;
    }

    public static WebApplication AfterProgramcsMiddleware(this WebApplication app)
    {
        app.StartAsync();
        return app;
    }

    public static HttpClient CreateHttpClientWithDefaultTestJwt(this WebApplication app)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6InVzZXItMSJ9.LUnCy-TvtvyRhFyyg2qFFwhGMLYAFFFqrKEcBLFAf1Q");
        return client;
    }
}