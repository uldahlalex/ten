using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

public class EmptyEnvironment(MyDbContext ctx) : ISeeder
{
    public void SeedDatabase()
    {
        ctx.Database.EnsureCreated();

      
            if (ctx.TaskTags.Any()) ctx.TaskTags.RemoveRange(ctx.TaskTags);
            if (ctx.Tickticktasks.Any()) ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
            if (ctx.Tasklists.Any()) ctx.Tasklists.RemoveRange(ctx.Tasklists);
            if (ctx.Tags.Any()) ctx.Tags.RemoveRange(ctx.Tags);
            if (ctx.Users.Any()) ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
     
        
    }
}