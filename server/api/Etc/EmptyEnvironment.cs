using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

public class EmptyEnvironment : ISeeder
{
    public void CreateEnvironment(MyDbContext ctx)
    {
        ctx.Database.EnsureDeleted(); //only use this if using testcontainers
        ctx.Database.EnsureCreated();

        // Clear existing data
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);
    }
}