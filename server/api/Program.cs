using System.Net;
using System.Text.Json;
using api;
using api.Seeder;
using Infrastructure.Postgres.Scaffolding;
using LightQuery.NSwag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

public class Program
{
    public static int DefaultPort { get; set; } = 8080;
    public static string? FinalBaseUrl { get; private set; }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISecurityService, SecurityService>();


        var thisAssembly = typeof(Program).Assembly;
        builder.Services.AddScoped<ITaskService, TaskService>();

        builder.Services.AddControllers()
        // .AddJsonOptions(opts =>
        // {
        //     opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        .AddApplicationPart(thisAssembly);  
        // //refernce handler no ciruclar references

        
        builder.Services.AddOpenApiDocument(conf =>
        {
            
            conf.OperationProcessors.Add(new LightQueryOperationsProcessor());

        });
        var appOptions = builder.Services.AddAppOptions(builder.Configuration);
        Console.WriteLine("App options: " + JsonSerializer.Serialize(appOptions));
        builder.Services.AddDbContext<MyDbContext>(ctx => 
            { ctx.UseNpgsql(appOptions.DbConnectionString); });
        builder.Services.AddScoped<ISeeder, DefaultEnvironment>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.WebHost.ConfigureKestrel(options =>
        {
            var port = DefaultPort;
            bool portFound = false;
    
            while (!portFound && port < DefaultPort + 100) // Limit the search to avoid infinite loop
            {
                try
                {
                    options.Listen(IPAddress.Any, port, listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                    });
                    portFound = true;
                    DefaultPort = port; // Update the DefaultPort to the one that was found
                }
                catch (IOException) // Port in use
                {
                    port++;
                }
            }

            if (!portFound)
            {
                throw new Exception($"Could not find an available port between {DefaultPort} and {DefaultPort + 100}");
            }
        });
    }

    public static void ConfigureApp(WebApplication app)
    {
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
                var schema = ctx.Database.GenerateCreateScript();
                File.WriteAllText("schema_according_to_dbcontext.sql", schema);
                scope.ServiceProvider.GetRequiredService<ISeeder>().CreateEnvironment(ctx).Wait();
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
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        
        var app = builder.Build();
        ConfigureApp(app);
        
        await app.StartAsync();
        FinalBaseUrl = app.Urls.First();
        await app.WaitForShutdownAsync();
    }
}