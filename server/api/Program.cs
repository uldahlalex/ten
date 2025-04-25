using api;
using api.Seeder;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<ISecurityService, SecurityService>();

        builder.Services.AddControllers();
        builder.Services.AddOpenApiDocument(conf => { });
        var appOptions = builder.Services.AddAppOptions(builder.Configuration);
        builder.Services.AddDbContext<MyDbContext>(ctx => { ctx.UseNpgsql(appOptions.DbConnectionString); });
        builder.Services.AddSingleton<IDefaultSeeder, DefaultSeeder>();

        var app = builder.Build();

        app.UseOpenApi(conf => { conf.Path = "openapi/v1.json"; });
        app.MapControllers();
        await app.GenerateTypeScriptClient("/../client/src/generated-client.ts");
        app.MapScalarApiReference();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        using (var scope = app.Services.CreateScope())
        {
            if (!app.Environment.IsProduction())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                await scope.ServiceProvider.GetRequiredService<IDefaultSeeder>().CreateEnvironment(ctx);
            }
        }

        app.Run();
    }
}