using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using api.Etc;
using api.Models;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation;
using AuthenticationService = api.Services.AuthenticationService;
using IAuthenticationService = api.Services.IAuthenticationService;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddScoped<ICryptographyService, CryptographyService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITotpService, TotpService>();

        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<ITaskDataService, TaskDataService>();

        services.AddScoped<ITaskService, TaskService>();
        services.AddControllers();
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });

        services.AddDbContext<MyDbContext>((provider, options) =>
        {
            //options.UseNpgsql(provider.GetRequiredService<AppOptions>().DbConnectionString);
            options.UseSqlite("Data Source=tasks.db");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        services.AddSingleton<ITestDataIds, TestDataIds>();
        services.AddTransient<ISeeder, TestDataSeeder>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddProblemDetails();
        services.AddOpenApiDocument(config =>
        {
            config.Title = "Task Management API";
            config.Version = "v1";
            
            // Add JWT Bearer authentication
            config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
            {
                Type = NSwag.OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter JWT token"
            });
            
            // Apply JWT security to all operations by default
            config.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

    }

    public static void ConfigureApp(WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<AppOptions>();
        Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);
        Console.WriteLine(JsonSerializer.Serialize(appOptions));
        // CORS should come before other middleware
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        
        // OpenAPI and Swagger UI middleware should come early
        app.UseOpenApi();
        app.UseSwaggerUi(conf =>
        {
            conf.WithCredentials = true;
        });

        app.UseMiddleware<CustomAuthMiddlewareSync<IJwtService>>();

        // Map controllers at the end
        app.MapControllers();

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
    
   


    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        

        var app = builder.Build();
    
        ConfigureApp(app);

        await app.RunAsync();
    }
}