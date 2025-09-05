using api;
using api.Etc;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ditests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        Program.ConfigureServices(services);
        // services.RemoveAll(typeof(TimeProvider));
        // services.AddSingleton<TimeProvider>();
        
        
        
        services.RemoveAll(typeof(MyDbContext));
        services.AddScoped<MyDbContext>((provider) =>
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new MyDbContext(options);
            context.Database.EnsureCreated();
            return context;
        });

    }
}