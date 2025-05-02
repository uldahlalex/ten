using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PgCtx;

namespace tests;

public static class ApiTestSetupUtilities
{
    public static WebApplicationBuilder DefaultTestConfig(
        this WebApplicationBuilder builder,
        bool useTestContainer = false
    )
    {
        var appOptions = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;
        if (useTestContainer || appOptions.RunsOn=="GitHub")
        {
            builder.Services.RemoveAll<MyDbContext>();
            builder.Services.AddDbContext<MyDbContext>(opt => opt.UseNpgsql(new PgCtxSetup<MyDbContext>()._postgres.GetConnectionString()));
        }
  
        builder.Services.RemoveAll<IWebHostPortAllocationService>();
        builder.Services.AddSingleton<IWebHostPortAllocationService, TestPortAllocationService>();
        return builder;
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