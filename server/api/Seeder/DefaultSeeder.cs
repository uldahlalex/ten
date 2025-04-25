using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Seeder;

public interface IDefaultSeeder
{
    Task CreateEnvironment(MyDbContext ctx);
}

public class DefaultSeeder : IDefaultSeeder
{
    public async Task CreateEnvironment(MyDbContext ctx)
    {
        ctx.Database.EnsureCreated();
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);
        ctx.Users.Add(new User()
        {
            UserId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            Email = "test@user.dk",
            Role = "User",
            Salt = "6cbf8487-8f2c-48bc-a15c-33d88eae8b9e",
            PasswordHash = "TJlSDc2mvpBmYKoi+2hnIOFx6ykf/V6JpmU7irhpoRcDT3KKUMwH7BWL/WlDTGrL11ud+5Q1BNxBEy3ZD1RRuQ==",


        });
        ctx.SaveChanges();

        var taskList = new List<Tasklist>()
        {
            new Tasklist()
            {
                UserId = ctx.Users.First().UserId,
                ListId = Guid.NewGuid().ToString(),
                Name = "Test List",
                CreatedAt = DateTime.UtcNow,
                Tickticktasks = new List<Tickticktask>()
                {
                    new Tickticktask()
                    {
                        TaskId = Guid.NewGuid().ToString(),
                        Title = "Test Task",
                        Description = "Test Description",
                        DueDate = DateTime.UtcNow,
                        Priority = 1,
                        CreatedAt = DateTime.UtcNow,

                    }
                }
            }
        };
        ctx.Tasklists.AddRange(taskList);
        ctx.SaveChanges();
            var Tags = new List<Tag>()
            {
                new Tag()
                {
                    
                    TagId = "test-tag-id",
                    Name = "Test Tag",
                    CreatedAt = DateTime.UtcNow,
                    UserId = ctx.Users.First().UserId,
                }
            };
        ctx.Tags.AddRange(Tags);
        ctx.SaveChanges();

    ctx.TaskTags.Add(new TaskTag()
        {
            CreatedAt = DateTime.UtcNow,
            TagId = ctx.Tags.First().TagId,
            TaskId = ctx.Tickticktasks.First().TaskId
        });

        ctx.SaveChanges();
    }
}