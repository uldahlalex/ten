using api;
using api.Etc;
using DotNet.Testcontainers.Builders;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace ditests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        Program.ConfigureServices(services);        
        services.RemoveAll(typeof(MyDbContext));
        services.AddScoped<MyDbContext>((provider) =>
        {
            
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
             postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
            var conn= postgreSqlContainer.GetConnectionString();
            var opts = new DbContextOptionsBuilder<MyDbContext>()
                .UseNpgsql(conn)
                .Options;
            var context = new MyDbContext(opts);
            context.Database.EnsureCreated();
            return context;
        });

    }
}