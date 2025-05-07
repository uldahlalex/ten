using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models;
using efscaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PgCtx;

namespace tests;

public static class ApiTestSetupUtilities
{
    public static WebApplicationBuilder DefaultTestConfig(
        this WebApplicationBuilder builder,
        bool useTestContainer = true
    )
    {
        builder.Environment.EnvironmentName = "Development";
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
    
}