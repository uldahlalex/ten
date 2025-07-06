using System.Text.Json;
using api.Etc;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace api;

public class Program
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {

        builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
        // Level 0: Foundation services (no dependencies)
        builder.Services.AddScoped<ICryptographyService, CryptographyService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<ITotpService, TotpService>();
        
        // Level 1: Data services (depend on DbContext + Level 0)
        builder.Services.AddScoped<IUserDataService, UserDataService>();
        builder.Services.AddScoped<ITaskDataService, TaskDataService>();
        
        // Level 2: Business services (depend on Level 0 + Level 1)
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        builder.Services.AddOpenApiDocument(conf =>
        {
            conf.Title = "Alex' Amazing REST API for training (loosely based on the 'TickTick' Task manager app)";
            conf.AddTypeToSwagger<ProblemDetails>();
        });
        builder.Services.AddSwaggerWithXmlDocs();
        var appOptions = builder.Services.AddAppOptions(builder.Configuration);
 
        builder.Services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(appOptions.DbConnectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        builder.Services.AddSingleton<ITestDataIds, TestDataIds>();
builder.Services.AddTransient<ISeeder, TestDataSeeder>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddSingleton<IWebHostPortAllocationService, ProductionPortAllocationService>();
    }

    public static WebApplication ConfigureApp(WebApplication app)
    {
        var portService = app.Services.GetRequiredService<IWebHostPortAllocationService>();
        app.Urls.Clear();
        app.Urls.Add(portService.GetBaseUrl());
        app.UseExceptionHandler();
        app.UseOpenApi(conf => { conf.Path = "/openapi/v1.json"; });
        app.UseSwaggerUi(conf =>
        {
            conf.Path = "/swagger";
            conf.DocumentPath = "/openapi/v1.json";
        });
        
        // Serve static files from wwwroot (React build output) or client folder in development
        if (app.Environment.IsDevelopment())
        {
            var clientPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "../../client/dist"));
            app.Services.GetRequiredService<ILogger<Program>>().LogInformation($"Looking for client files at: {clientPath}");
            
            if (Directory.Exists(clientPath))
            {
                app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Serving React app from client/dist folder");
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientPath),
                    RequestPath = ""
                });
            }
            else
            {
                app.Services.GetRequiredService<ILogger<Program>>().LogWarning($"Client dist folder not found at: {clientPath}");
                app.UseStaticFiles(); // Fallback to wwwroot
            }
        }
        else
        {
            app.UseStaticFiles();
        }
        
        app.MapControllers();
        app.GenerateApiClientsFromOpenApi("/../../client/src/models/generated-client.ts").Wait();
        app.MapScalarApiReference();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        var environment = app.Environment.EnvironmentName;
        app.Services.GetRequiredService<ILogger<Program>>().LogInformation("ENV: " + environment);

        using (var scope = app.Services.CreateScope())
        {
            if (!app.Environment.IsProduction())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var schema = ctx.Database.GenerateCreateScript();
                File.WriteAllText("schema_according_to_dbcontext.sql", schema);
                scope.ServiceProvider.GetRequiredService<ISeeder>().SeedDatabase();
            }
        }

        // Fallback routing for React Router (SPA)
        if (app.Environment.IsDevelopment())
        {
            var clientPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "../../client/dist"));
            if (Directory.Exists(clientPath))
            {
                app.MapFallbackToFile("index.html", new StaticFileOptions
                {
                    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientPath)
                });
            }
            else
            {
                app.MapFallbackToFile("index.html");
            }
        }
        else
        {
            app.MapFallbackToFile("index.html");
        }

        return app;
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);

        var app = builder.Build();
        ConfigureApp(app);

        await app.RunAsync();
    }
}