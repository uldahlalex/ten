using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

/// <summary>
/// Factory for creating Users with dependency injection support
/// </summary>
public class UserFactory
{
    private readonly ISecurityService _securityService;
    private readonly TimeProvider _timeProvider;

    public UserFactory(ISecurityService securityService, TimeProvider timeProvider)
    {
        _securityService = securityService;
        _timeProvider = timeProvider;
    }

    public User Create(string id, string email, int daysAgo = 0)
    {
        return CreateAtTime(id, email, _timeProvider.GetUtcNow().DateTime.AddDays(-daysAgo));
    }

    public User CreateAtTime(string id, string email, DateTime createdAt)
    {
        return new User(
            email: email,
            salt: "test-salt",
            passwordHash: "test-hash",
            role: Role.User,
            totpSecret: _securityService.GenerateSecretKey(),
            createdAt: createdAt,
            userId: id
        );
    }
}

/// <summary>
/// Factory for creating Tags with dependency injection support
/// </summary>
public class TagFactory
{
    private readonly TimeProvider _timeProvider;

    public TagFactory(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Tag Create(string id, string name, string userId, int daysAgo = 0)
    {
        return CreateAtTime(id, name, userId, _timeProvider.GetUtcNow().DateTime.AddDays(-daysAgo));
    }

    public Tag CreateAtTime(string id, string name, string userId, DateTime createdAt)
    {
        return new Tag(
            createdAt: createdAt,
            name: name,
            userId: userId,
            id: id
        );
    }
}

/// <summary>
/// Factory for creating Tasklists with dependency injection support
/// </summary>
public class TasklistFactory
{
    private readonly TimeProvider _timeProvider;

    public TasklistFactory(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Tasklist Create(string id, string name, string userId, int daysAgo = 0)
    {
        return CreateAtTime(id, name, userId, _timeProvider.GetUtcNow().DateTime.AddDays(-daysAgo));
    }

    public Tasklist CreateAtTime(string id, string name, string userId, DateTime createdAt)
    {
        return new Tasklist(
            createdAt: createdAt,
            name: name,
            userId: userId,
            id: id
        );
    }
}

/// <summary>
/// Factory for creating Tasks with dependency injection support
/// </summary>
public class TaskFactory
{
    private readonly TimeProvider _timeProvider;

    public TaskFactory(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Tickticktask Create(string id, string title, string listId, string? description = null,
        int priority = 1, bool completed = false, int createdDaysAgo = 0, 
        int? dueDaysFromNow = null, int? completedDaysAgo = null)
    {
        var baseTime = _timeProvider.GetUtcNow().DateTime;
        
        return CreateAtTime(
            id: id,
            title: title,
            listId: listId,
            description: description,
            priority: priority,
            completed: completed,
            createdAt: baseTime.AddDays(-createdDaysAgo),
            dueDate: dueDaysFromNow.HasValue ? baseTime.AddDays(dueDaysFromNow.Value) : null,
            completedAt: completedDaysAgo.HasValue ? baseTime.AddDays(-completedDaysAgo.Value) : null
        );
    }

    public Tickticktask CreateAtTime(string id, string title, string listId, string? description = null,
        int priority = 1, bool completed = false, DateTime? createdAt = null, 
        DateTime? dueDate = null, DateTime? completedAt = null)
    {
        return new Tickticktask(
            createdAt: createdAt ?? _timeProvider.GetUtcNow().DateTime,
            listId: listId,
            title: title,
            description: description,
            dueDate: dueDate,
            priority: priority,
            completed: completed,
            completedAt: completedAt,
            taskId: id
        );
    }
}

/// <summary>
/// Central test data registry using DI factories
/// </summary>
public class TestDataRegistry(
    UserFactory userFactory,
    TagFactory tagFactory,
    TasklistFactory tasklistFactory,
    TaskFactory taskFactory,
    TimeProvider timeProvider)
{
    private DateTime BaseTime => timeProvider.GetUtcNow().UtcDateTime;
    
    // Users - using BaseTime for consistency
    public User John => userFactory.CreateAtTime("john-001", "john@example.com", BaseTime.AddDays(-30));
    public User Jane => userFactory.CreateAtTime("jane-002", "jane@example.com", BaseTime.AddDays(-25));
    public User Admin => userFactory.CreateAtTime("admin-003", "admin@example.com", BaseTime.AddDays(-35));

    // Tags - using BaseTime for consistency
    public Tag UrgentTag => tagFactory.CreateAtTime("tag-urgent", "Urgent", John.UserId, BaseTime.AddDays(-25));
    public Tag BugTag => tagFactory.CreateAtTime("tag-bug", "Bug", John.UserId, BaseTime.AddDays(-25));
    public Tag FeatureTag => tagFactory.CreateAtTime("tag-feature", "Feature", John.UserId, BaseTime.AddDays(-25));
    public Tag ImportantTag => tagFactory.CreateAtTime("tag-important", "Important", John.UserId, BaseTime.AddDays(-25));
    public Tag PersonalTag => tagFactory.CreateAtTime("tag-personal", "Personal", Jane.UserId, BaseTime.AddDays(-20));

    // Task Lists - using BaseTime for consistency
    public Tasklist WorkList => tasklistFactory.CreateAtTime("list-work", "Work Tasks", John.UserId, BaseTime.AddDays(-20));
    public Tasklist PersonalList => tasklistFactory.CreateAtTime("list-personal", "Personal Tasks", John.UserId, BaseTime.AddDays(-20));
    public Tasklist JanePersonalList => tasklistFactory.CreateAtTime("list-jane-personal", "Jane's Tasks", Jane.UserId, BaseTime.AddDays(-18));
    public Tasklist ShoppingList => tasklistFactory.CreateAtTime("list-shopping", "Shopping", John.UserId, BaseTime.AddDays(-15));

    // Tasks - using BaseTime for consistency
    public Tickticktask CriticalBugTask => taskFactory.CreateAtTime(
        "task-critical-bug", "Fix critical login bug", WorkList.ListId,
        "Users cannot log in, blocking all functionality", 
        priority: 5, createdAt: BaseTime.AddDays(-5), dueDate: BaseTime.AddDays(1));

    public Tickticktask SearchFeatureTask => taskFactory.CreateAtTime(
        "task-search", "Implement search feature", WorkList.ListId,
        "Add full-text search to dashboard", 
        priority: 3, createdAt: BaseTime.AddDays(-3), dueDate: BaseTime.AddDays(7));

    public Tickticktask UpdateDocsTask => taskFactory.CreateAtTime(
        "task-docs", "Update API documentation", WorkList.ListId,
        "Update OpenAPI specs for new endpoints", 
        priority: 2, completed: true, createdAt: BaseTime.AddDays(-4), 
        dueDate: BaseTime.AddDays(3), completedAt: BaseTime.AddDays(-1));

    public Tickticktask GroceriesTask => taskFactory.CreateAtTime(
        "task-groceries", "Buy groceries", PersonalList.ListId,
        "Weekly shopping: milk, bread, eggs", 
        priority: 2, createdAt: BaseTime.AddDays(-1), dueDate: BaseTime.AddDays(2));

    public Tickticktask DentistTask => taskFactory.CreateAtTime(
        "task-dentist", "Schedule dentist appointment", JanePersonalList.ListId,
        "Annual checkup and cleaning", 
        priority: 1, createdAt: BaseTime.AddDays(-2), dueDate: BaseTime.AddDays(5));

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
}

/// <summary>
/// Simple seeder that uses the DI-based test data registry
/// </summary>
public class TestEnvironment(TestDataRegistry Data, ILogger<TestEnvironment> logger, TimeProvider timeProvider) : ISeeder
{

    public void SeedDatabase(MyDbContext ctx)
    {
        ctx.Database.EnsureCreated();
        ClearDatabase(ctx);

        // Create users
        var users = new[] { Data.John, Data.Jane, Data.Admin };
        ctx.Users.AddRange(users);
        ctx.SaveChanges();

        // Create tags
        var tags = new[] 
        { 
            Data.UrgentTag, Data.BugTag, Data.FeatureTag, 
            Data.ImportantTag, Data.PersonalTag 
        };
        ctx.Tags.AddRange(tags);
        ctx.SaveChanges();

        // Create task lists
        var tasklists = new[] 
        { 
            Data.WorkList, Data.PersonalList, 
            Data.JanePersonalList, Data.ShoppingList 
        };
        ctx.Tasklists.AddRange(tasklists);
        ctx.SaveChanges();

        // Create tasks
        var tasks = new[] 
        { 
            Data.CriticalBugTask, Data.SearchFeatureTask, Data.UpdateDocsTask,
            Data.GroceriesTask, Data.DentistTask 
        };
        ctx.Tickticktasks.AddRange(tasks);
        ctx.SaveChanges();

        // Create task-tag relationships
        var taskTags = Data.TaskTagPairs.Select(pair => 
            new TaskTag(
                createdAt: timeProvider.GetUtcNow().UtcDateTime.AddDays(-3),
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

