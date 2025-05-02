using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Seeder;

public interface ISeeder
{
    Task CreateEnvironment(MyDbContext ctx);
}

public class EmptyEnvironment : ISeeder
{
    public async Task CreateEnvironment(MyDbContext ctx)
    {
        ctx.Database.EnsureCreated();
        
        // Clear existing data
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);
    }
}

/// <summary>
/// Will seed the following data:
/// 1 user (test@user.dk, password abc)
/// lists: "Work", "Personal", "Shopping", "Health", "Learning","Project A", "Project B", "Maintenance", "Ideas", "Goals"
/// tags: "urgent", "important", "can-wait", "bug", "feature", "documentation", "testing", "research", "meeting", "follow-up", "blocked", "in-progress", "review", "approved", "deployed"
/// 15 tasks per list with random titles and descriptions and 1 tag each
/// </summary>
public class DefaultEnvironment : ISeeder
{
    private static readonly string[] TaskTitles = {
        "Complete project documentation", "Review pull requests", "Setup development environment",
        "Fix critical bug", "Update dependencies", "Write unit tests", "Deploy to production",
        "Create backup", "Optimize database queries", "Implement new feature", "Code review",
        "Security audit", "Performance testing", "User acceptance testing", "Release planning"
    };

    private static readonly string[] TaskDescriptions = {
        "High priority task that needs immediate attention",
        "Regular maintenance task for system stability",
        "Feature enhancement requested by client",
        "Bug fix for reported issue in production",
        "Documentation update for new features"
    };

    private static readonly string[] ListNames = {
        "Work", "Personal", "Shopping", "Health", "Learning",
        "Project A", "Project B", "Maintenance", "Ideas", "Goals"
    };

    private static readonly string[] TagNames = {
        "urgent", "important", "can-wait", "bug", "feature",
        "documentation", "testing", "research", "meeting", "follow-up",
        "blocked", "in-progress", "review", "approved", "deployed"
    };


    public async Task CreateEnvironment(MyDbContext ctx)
    {
        Console.WriteLine("now this runs");
        Console.WriteLine(ctx.Database.GetConnectionString());
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        
        // Clear existing data
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);

        // Create test user
        var user = new User
        {
            UserId = "user-1",
            CreatedAt = DateTime.UtcNow,
            Email = "test@user.dk",
            Role = "User",
            Salt = "6cbf8487-8f2c-48bc-a15c-33d88eae8b9e",
            PasswordHash = "TJlSDc2mvpBmYKoi+2hnIOFx6ykf/V6JpmU7irhpoRcDT3KKUMwH7BWL/WlDTGrL11ud+5Q1BNxBEy3ZD1RRuQ=="
        };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        // Create tags
        var tags = TagNames.Select(name => new Tag
        {
            TagId = $"tag-{name}",
            Name = name,
            CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
            UserId = user.UserId
        }).ToList();
        ctx.Tags.AddRange(tags);
        await ctx.SaveChangesAsync();

        // Create lists with tasks
        var lists = ListNames.Select(name => new Tasklist
        {
            ListId = $"list-{name}",
            Name = name,
            CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
            UserId = user.UserId
        }).ToList();

        foreach (var list in lists)
        {
            var tasks = new List<Tickticktask>();

            for (int i = 0; i < 15; i++)
            {
                var completed = i % 2 == 0;
                var task = new Tickticktask
                {
                    TaskId = $"task-{i}-list-{list.ListId}",
                    Title = TaskTitles[i % TaskTitles.Length],
                    Description = TaskDescriptions[i % TaskDescriptions.Length],
                    DueDate = DateTime.UtcNow.AddDays(i),
                    Priority = 5 % (i+1),
                    Completed = completed,
                    CreatedAt = DateTime.UtcNow.AddDays(i),
                    CompletedAt = completed ? DateTime.UtcNow.Subtract(TimeSpan.FromHours(i))  : null,
                    ListId = list.ListId
                };
                tasks.Add(task);
            }

            list.Tickticktasks = tasks;
        }

        ctx.Tasklists.AddRange(lists);
        await ctx.SaveChangesAsync();

        // Add random tags to tasks (2-4 tags per task)
        var allTasks = lists.SelectMany(l => l.Tickticktasks).ToList();
        var taskTags = new List<TaskTag>();

        var taskIndex = 0;
        foreach (var task in allTasks)
        {

         
                task.TaskTags.Add(new TaskTag
                {
                    TaskId = task.TaskId,
                    TagId = tags[taskIndex % tags.Count].TagId,
                    CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                });
                taskIndex++;
        }

        ctx.TaskTags.AddRange(taskTags);
        await ctx.SaveChangesAsync();
        var count = ctx.Tickticktasks.Count();
        Console.WriteLine(count);
        if (count == 0)
            throw new Exception("No tasks found in DB");
    }
}