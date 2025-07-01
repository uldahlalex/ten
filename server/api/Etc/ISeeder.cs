using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

public interface ISeeder
{
    void SeedDatabase(MyDbContext ctx);
}