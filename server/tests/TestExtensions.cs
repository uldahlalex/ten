using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        bool useTestContainer = true
    )
    {
        if (useTestContainer)
        {
            var db = new PgCtxSetup<MyDbContext>();
            RemoveExistingService<MyDbContext>(services);
            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(db._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging();
                opt.LogTo(_ => { });
            });
        }
        return services;
    }

    private static void RemoveExistingService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    public static async Task<string> TestRegisterAndAddJwt(this HttpClient httpClient, string baseUrl)
    {
        var registerDto = new AuthRequestDto
        {
            Email = new Random().NextDouble() * 123 + "@gmail.com",
            Password = new Random().NextDouble() * 123 + "@gmail.com"
        };
        var route = baseUrl +
                    nameof(AuthController.Register);
        var signIn = await httpClient.PostAsJsonAsync(route, registerDto);
        if (!signIn.IsSuccessStatusCode)
            throw new Exception("Sign up failed!: "+await signIn.Content.ReadAsStringAsync());
        var jwt = await signIn.Content
            .ReadAsStringAsync();
        if (string.IsNullOrEmpty(jwt))
            throw new Exception("No jwt!");
        httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(jwt);
        return jwt;
    }

  
    
}