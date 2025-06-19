using System.Text.Json;
using api.Etc;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PgCtx;
using Scalar.AspNetCore;

namespace api;

public class Program
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        builder.Services.AddOpenApiDocument(conf =>
        {
            conf.Title = "Alex' Amazing REST API for training (loosely based on the 'TickTick' Task manager app)";
            conf.AddTypeToSwagger<ProblemDetails>();
        });
        builder.Services.AddSwaggerWithXmlDocs();
        var appOptions = builder.Services.AddAppOptions(builder.Configuration);
        Console.WriteLine("App options: " + JsonSerializer.Serialize(appOptions));
        var isGithubActions = appOptions.RunsOn == "GitHub";
        if (!isGithubActions)
        {
            var pgctx = new PgCtxSetup<MyDbContext>();
            appOptions.DbConnectionString = pgctx._postgres.GetConnectionString();
        }
        builder.Services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(appOptions.DbConnectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        builder.Services.AddScoped<ISeeder, DefaultEnvironment>();
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
        app.MapControllers();
        app.GenerateTypeScriptClientFromOpenApi("/../../client/src/generated-client.ts").Wait();
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
                scope.ServiceProvider.GetRequiredService<ISeeder>().CreateEnvironment(ctx);
            }
        }

        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == 404)
            {
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Route not found",
                    Detail = $"The route {context.Request.Path} does not exist (no controller methods match)",
                    Status = StatusCodes.Status404NotFound
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        });

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