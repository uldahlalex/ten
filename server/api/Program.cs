using System.Text.Json;
using api;
using api.Seeder;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PgCtx;
using Scalar.AspNetCore;

public class Program
{
    public static int DefaultPort { get; set; } = 8080;
    public static string? FinalBaseUrl { get; private set; }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        builder.Services.AddOpenApiDocument(conf =>
        {
        });
        var appOptions = builder.Services.AddAppOptions(builder.Configuration);
        Console.WriteLine("App options: " + JsonSerializer.Serialize(appOptions));
        builder.Services.AddDbContext<MyDbContext>(ctx =>
        {
            ctx.UseNpgsql(appOptions.DbConnectionString);
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
        app.GenerateTypeScriptClient("/../client/src/generated-client.ts").Wait();
        app.MapScalarApiReference();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        using (var scope = app.Services.CreateScope())
        {
            if (!app.Environment.IsProduction())
            {
                MyDbContext ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                Console.WriteLine(ctx.Database.GetConnectionString());

                var schema = ctx.Database.GenerateCreateScript();
                File.WriteAllText("schema_according_to_dbcontext.sql", schema);
                scope.ServiceProvider.GetRequiredService<ISeeder>().CreateEnvironment(ctx);
            }
        }
        app.MapGet("/helloworld", () => "Hello World!");
        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == 404)
            {
                context.Response.ContentType = "application/json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = $"The route {context.Request.Path} does not exist",
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