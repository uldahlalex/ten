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
            RemoveExistingService<MyDbContext>(services);
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

    public static async Task<string> TestRegisterAndAddJwt(this HttpClient httpClient, string baseUrl)
    {
        var registerDto = new AuthRequestDto
        {
            Email = new Random().NextDouble() * 123 + "@gmail.com",
            Password = new Random().NextDouble() * 123 + "@gmail.com"
        };
        var route = baseUrl +
                    AuthController.RegisterRoute;
        Console.WriteLine(route);
        var signIn = await httpClient.PostAsJsonAsync(route, registerDto);
        if (!signIn.IsSuccessStatusCode)
            throw new Exception("Sign up failed!: "+await signIn.Content.ReadAsStringAsync());
        var jwt = await signIn.Content
            .ReadAsStringAsync();
        if (string.IsNullOrEmpty(jwt))
            throw new Exception("No jwt!");
        httpClient.DefaultRequestHeaders.Add("Authorization", jwt);
        return jwt;
    }

  
        public static string ToQueryString(this TaskQueryParams queryParams)
        {
            var queryParts = new List<string>();

            if (queryParams.IsCompleted.HasValue)
                queryParts.Add($"{nameof(TaskQueryParams.IsCompleted)}={queryParams.IsCompleted.Value}");

            if (queryParams.DueDateStart.HasValue)
                queryParts.Add($"{nameof(TaskQueryParams.DueDateStart)}={Uri.EscapeDataString(queryParams.DueDateStart.Value.ToString("O"))}");

            if (queryParams.DueDateEnd.HasValue)
                queryParts.Add($"{nameof(TaskQueryParams.DueDateEnd)}={Uri.EscapeDataString(queryParams.DueDateEnd.Value.ToString("O"))}");

            if (queryParams.MinPriority.HasValue)
                queryParts.Add($"{nameof(TaskQueryParams.MinPriority)}={queryParams.MinPriority.Value}");

            if (queryParams.MaxPriority.HasValue)
                queryParts.Add($"{nameof(TaskQueryParams.MaxPriority)}={queryParams.MaxPriority.Value}");

            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                queryParts.Add($"{nameof(TaskQueryParams.SearchTerm)}={Uri.EscapeDataString(queryParams.SearchTerm)}");

            if (queryParams.TagIds?.Any() == true)
                queryParts.Add($"{nameof(TaskQueryParams.TagIds)}={string.Join(",", queryParams.TagIds.Select(Uri.EscapeDataString))}");

            if (queryParams.ListIds?.Any() == true)
                queryParts.Add($"{nameof(TaskQueryParams.ListIds)}={string.Join(",", queryParams.ListIds.Select(Uri.EscapeDataString))}");

            if (queryParams.OrderBy != null)
                queryParts.Add($"{nameof(TaskQueryParams.OrderBy)}={Uri.EscapeDataString(queryParams.OrderBy.Value)}");
            
            queryParts.Add($"{nameof(TaskQueryParams.IsDescending)}={queryParams.IsDescending}");

            return queryParts.Any() ? "?" + string.Join("&", queryParts) : "";
        }
    
}