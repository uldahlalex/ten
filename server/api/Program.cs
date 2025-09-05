using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        // Level 0: Foundation services (no dependencies)
        services.AddScoped<ICryptographyService, CryptographyService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITotpService, TotpService>();

        // Level 1: Data services (depend on DbContext + Level 0)
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<ITaskDataService, TaskDataService>();

        // Level 2: Business services (depend on Level 0 + Level 1)
        services.AddScoped<ITaskService, TaskService>();
        services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        services.AddOpenApiDocument(conf =>
        {
            conf.Title = "Alex' Amazing REST API for training (loosely based on the 'TickTick' Task manager app)";
            conf.AddTypeToSwagger<ProblemDetails>();
            conf.SchemaSettings.AlwaysAllowAdditionalObjectProperties = false;
            conf.SchemaSettings.GenerateAbstractProperties = true;
            conf.SchemaSettings.SchemaProcessors.Add(new RequiredSchemaProcessor());
        });
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });

        services.AddDbContext<MyDbContext>((provider, options) =>
        {
            options.UseNpgsql(provider.GetRequiredService<AppOptions>().DbConnectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        services.AddSingleton<ITestDataIds, TestDataIds>();
        services.AddTransient<ISeeder, TestDataSeeder>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        // services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddSingleton<IWebHostPortAllocationService, ProductionPortAllocationService>();
    }

    public static void ConfigureApp(WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<AppOptions>();
        //Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);

        // var portService = app.Services.GetRequiredService<IWebHostPortAllocationService>();
        // app.Urls.Clear();
        // app.Urls.Add(portService.GetBaseUrl());
        app.UseExceptionHandler();
        app.UseOpenApi();
        app.UseSwaggerUi();

        app.MapControllers();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        //Pesist the Openapi.json to the local file system for reference
        app.Lifetime.ApplicationStarted.Register(async () =>
        {
            var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
                .GenerateAsync("v1");
            var openApiJson = document.ToJson();
            var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "openapi.json");
            await File.WriteAllTextAsync(openApiPath, openApiJson);
        });

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            ctx.Database.EnsureCreated();
            if (!app.Environment.IsProduction())
            {
                var schema = ctx.Database.GenerateCreateScript();
                File.WriteAllText("schema_according_to_dbcontext.sql", schema);
                scope.ServiceProvider.GetRequiredService<ISeeder>().SeedDatabase();
            }
        }
    }
    
    private static async Task GenerateOpenApiOnly(WebApplication app)
    {
        try
        {
            var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
                .GenerateAsync("v1");
            var openApiJson = document.ToJson();
            var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "openapi.json");
            await File.WriteAllTextAsync(openApiPath, openApiJson);
            Console.WriteLine($"OpenAPI spec generated successfully at: {openApiPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate OpenAPI spec: {ex.Message}");
            Environment.Exit(1);
        }
    }


    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        

        var app = builder.Build();
        if (args.Length > 0 && args[0] == "--generate-openapi-only")
        {
            await GenerateOpenApiOnly(app);
            return;
        }
        ConfigureApp(app);

        await app.RunAsync();
    }
}