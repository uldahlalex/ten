using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models;
using api.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var appOptions = new AppOptions();
            serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(AppOptions)).Bind(appOptions);
            services.AddSingleton<AppOptions>();
        }

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
        Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);

        var portService = app.Services.GetRequiredService<IWebHostPortAllocationService>();
        app.Urls.Clear();
        app.Urls.Add(portService.GetBaseUrl());
        app.UseExceptionHandler();
        app.UseOpenApi();
        app.UseSwaggerUi();

        app.MapControllers();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


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