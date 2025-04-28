using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Seeder;

public interface IDefaultSeeder
{
    Task CreateEnvironment(MyDbContext ctx);
}

public class DefaultSeeder : IDefaultSeeder
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

    private readonly Random _random = new Random();

    public async Task CreateEnvironment(MyDbContext ctx)
    {
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
            CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 60)),
            UserId = user.UserId
        }).ToList();
        ctx.Tags.AddRange(tags);
        await ctx.SaveChangesAsync();

        // Create lists with tasks
        var lists = ListNames.Select(name => new Tasklist
        {
            ListId = $"list-{name}",
            Name = name,
            CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 60)),
            UserId = user.UserId
        }).ToList();

        foreach (var list in lists)
        {
            var tasksCount = 15;
            var tasks = new List<Tickticktask>();

            for (int i = 0; i < tasksCount; i++)
            {
                var createdAt = DateTime.UtcNow.AddDays(-_random.Next(1, 90));
                var isCompleted = _random.Next(2) == 1;
                var completedAt = isCompleted ? 
                    createdAt.AddDays(_random.Next(1, 30)) : 
                    DateTime.UtcNow;

                var task = new Tickticktask
                {
                    TaskId = $"task-{i}-list-{list.ListId}",
                    Title = TaskTitles[_random.Next(TaskTitles.Length)],
                    Description = TaskDescriptions[_random.Next(TaskDescriptions.Length)],
                    DueDate = DateTime.UtcNow.AddDays(_random.Next(-30, 61)),
                    Priority = _random.Next(1, 5),
                    Completed = isCompleted,
                    CreatedAt = createdAt,
                    CompletedAt = completedAt,
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

        foreach (var task in allTasks)
        {
            var tagCount = _random.Next(2, 5);
            var selectedTags = tags.OrderBy(x => _random.Next()).Take(tagCount);

            foreach (var tag in selectedTags)
            {
                taskTags.Add(new TaskTag
                {
                    TaskId = task.TaskId,
                    TagId = tag.TagId,
                    CreatedAt = task.CreatedAt.AddMinutes(_random.Next(1, 60))
                });
            }
        }

        ctx.TaskTags.AddRange(taskTags);
        await ctx.SaveChangesAsync();
    }
}