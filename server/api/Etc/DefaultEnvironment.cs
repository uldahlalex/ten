using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

/// <summary>
/// Single source of truth for test data with direct object creation
/// </summary>
public class TestDataSeeder(ISecurityService securityService, TimeProvider timeProvider, ILogger<TestDataSeeder> logger)
    : ISeeder
{
    private DateTime BaseTime => timeProvider.GetUtcNow().UtcDateTime;

    // Test data properties - created on demand with consistent BaseTime
    public User John => new User(
        email: "john@example.com",
        salt: "test-salt",
        passwordHash: "test-hash",
        role: Role.User,
        totpSecret: securityService.GenerateSecretKey(),
        createdAt: BaseTime.AddDays(-30),
        userId: "john-001"
    );

    public User Jane => new User(
        email: "jane@example.com",
        salt: "test-salt",
        passwordHash: "test-hash",
        role: Role.User,
        totpSecret: securityService.GenerateSecretKey(),
        createdAt: BaseTime.AddDays(-25),
        userId: "jane-002"
    );

    public User Admin => new User(
        email: "admin@example.com",
        salt: "test-salt",
        passwordHash: "test-hash",
        role: Role.User,
        totpSecret: securityService.GenerateSecretKey(),
        createdAt: BaseTime.AddDays(-35),
        userId: "admin-003"
    );

    // Tags
    public Tag UrgentTag => new Tag(BaseTime.AddDays(-25), "Urgent", John.UserId, "tag-urgent");
    public Tag BugTag => new Tag(BaseTime.AddDays(-25), "Bug", John.UserId, "tag-bug");
    public Tag FeatureTag => new Tag(BaseTime.AddDays(-25), "Feature", John.UserId, "tag-feature");
    public Tag ImportantTag => new Tag(BaseTime.AddDays(-25), "Important", John.UserId, "tag-important");
    public Tag PersonalTag => new Tag(BaseTime.AddDays(-20), "Personal", Jane.UserId, "tag-personal");

    // Task Lists
    public Tasklist WorkList => new Tasklist(BaseTime.AddDays(-20), "Work Tasks", John.UserId, "list-work");
    public Tasklist PersonalList => new Tasklist(BaseTime.AddDays(-20), "Personal Tasks", John.UserId, "list-personal");
    public Tasklist JanePersonalList => new Tasklist(BaseTime.AddDays(-18), "Jane's Tasks", Jane.UserId, "list-jane-personal");
    public Tasklist ShoppingList => new Tasklist(BaseTime.AddDays(-15), "Shopping", John.UserId, "list-shopping");

    // Tasks
    public Tickticktask CriticalBugTask => new Tickticktask(
        createdAt: BaseTime.AddDays(-5),
        listId: WorkList.ListId,
        title: "Fix critical login bug",
        description: "Users cannot log in, blocking all functionality",
        dueDate: BaseTime.AddDays(1),
        priority: 5,
        completed: false,
        completedAt: null,
        taskId: "task-critical-bug"
    );

    public Tickticktask SearchFeatureTask => new Tickticktask(
        createdAt: BaseTime.AddDays(-3),
        listId: WorkList.ListId,
        title: "Implement search feature",
        description: "Add full-text search to dashboard",
        dueDate: BaseTime.AddDays(7),
        priority: 3,
        completed: false,
        completedAt: null,
        taskId: "task-search"
    );

    public Tickticktask UpdateDocsTask => new Tickticktask(
        createdAt: BaseTime.AddDays(-4),
        listId: WorkList.ListId,
        title: "Update API documentation",
        description: "Update OpenAPI specs for new endpoints",
        dueDate: BaseTime.AddDays(3),
        priority: 2,
        completed: true,
        completedAt: BaseTime.AddDays(-1),
        taskId: "task-docs"
    );

    public Tickticktask GroceriesTask => new Tickticktask(
        createdAt: BaseTime.AddDays(-1),
        listId: PersonalList.ListId,
        title: "Buy groceries",
        description: "Weekly shopping: milk, bread, eggs",
        dueDate: BaseTime.AddDays(2),
        priority: 2,
        completed: false,
        completedAt: null,
        taskId: "task-groceries"
    );

    public Tickticktask DentistTask => new Tickticktask(
        createdAt: BaseTime.AddDays(-2),
        listId: JanePersonalList.ListId,
        title: "Schedule dentist appointment",
        description: "Annual checkup and cleaning",
        dueDate: BaseTime.AddDays(5),
        priority: 1,
        completed: false,
        completedAt: null,
        taskId: "task-dentist"
    );

    // Task-Tag relationships
    public (Tickticktask Task, Tag Tag)[] TaskTagPairs => new[]
    {
        (CriticalBugTask, UrgentTag),
        (CriticalBugTask, BugTag),
        (SearchFeatureTask, FeatureTag),
        (SearchFeatureTask, ImportantTag),
        (GroceriesTask, ImportantTag),
        (DentistTask, PersonalTag)
    };

    public void SeedDatabase

    (MyDbContext ctx)
    {
        ctx.Database.EnsureCreated();
        ClearDatabase(ctx);

        // Create users
        var users = new[] { John, Jane, Admin };
        ctx.Users.AddRange(users);
        ctx.SaveChanges();

        // Create tags
        var tags = new[] { UrgentTag, BugTag, FeatureTag, ImportantTag, PersonalTag };
        ctx.Tags.AddRange(tags);
        ctx.SaveChanges();

        // Create task lists
        var tasklists = new[] { WorkList, PersonalList, JanePersonalList, ShoppingList };
        ctx.Tasklists.AddRange(tasklists);
        ctx.SaveChanges();

        // Create tasks
        var tasks = new[] { CriticalBugTask, SearchFeatureTask, UpdateDocsTask, GroceriesTask, DentistTask };
        ctx.Tickticktasks.AddRange(tasks);
        ctx.SaveChanges();

        // Create task-tag relationships
        var taskTags = TaskTagPairs.Select(pair => 
            new TaskTag(
                createdAt: BaseTime.AddDays(-3),
                taskId: pair.Task.TaskId,
                tagId: pair.Tag.TagId
            )).ToList();
            
        ctx.TaskTags.AddRange(taskTags);
        ctx.SaveChanges();

        logger.LogInformation("Test environment seeded with {UserCount} users, {TagCount} tags, {TaskCount} tasks",
            users.Length, tags.Length, tasks.Length);
    }

    private void ClearDatabase(MyDbContext ctx)
    {
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);
        ctx.SaveChanges();
    }
}

// Extension methods for easy querying (optional)
public static class TestDataQueries
{
    public static IEnumerable<Tickticktask> GetTasksWithTag(this MyDbContext ctx, Tag tag)
    {
        return from tt in ctx.TaskTags
               where tt.TagId == tag.TagId
               join t in ctx.Tickticktasks on tt.TaskId equals t.TaskId
               select t;
    }

    public static IEnumerable<Tag> GetTagsForTask(this MyDbContext ctx, Tickticktask task)
    {
        return from tt in ctx.TaskTags
               where tt.TaskId == task.TaskId
               join tag in ctx.Tags on tt.TagId equals tag.TagId
               select tag;
    }
}

