using System.Net.Http.Json;
using System.Text.Json;
using api;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PgCtx;

namespace tests;

public static class ApiTestSetupUtilities
{
    public static IServiceCollection DefaultTestConfig(
        this IServiceCollection services,
        bool useTestContainer = true,
        bool mockProxyConfig = true,
        bool makeWsClient = true,
        bool makeMqttClient = true,
        Action? customSeeder = null
    )
    {
        if (useTestContainer)
        {
            var db = new PgCtxSetup<MyDbContext>();
            RemoveExistingService<DbContextOptions<MyDbContext>>(services);
            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(db._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging();
                opt.LogTo(_ => { });
            });
        }

        if (mockProxyConfig)
        {
           
        }

        if (customSeeder is not null)
        {
           
        }

     

        return services;
    }

    private static void RemoveExistingService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }
    public static async Task<string> TestRegisterAndAddJwt(this HttpClient httpClient)
    {
        var registerDto = new AuthRequestDto
        {
            Email = new Random().NextDouble() * 123 + "@gmail.com",
            Password = new Random().NextDouble() * 123 + "@gmail.com"
        };
        var signIn = await httpClient.PostAsJsonAsync(
            AuthController.RegisterRoute, registerDto);
        var jwt = await signIn.Content
            .ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Add("Authorization", jwt);
        return jwt;
    }
    
}

