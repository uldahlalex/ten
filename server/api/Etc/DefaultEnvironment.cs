using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

public class SeededData
{
    public User User { get; set; } = null!;
    public List<Tag> Tags { get; set; } = new();
    public List<Tasklist> Lists { get; set; } = new();
    public List<Tickticktask> Tasks { get; set; } = new();
    public List<TaskTag> TaskTags { get; set; } = new();
    
    // Easy access properties
    public Tag UrgentTag => Tags.First(t => t.TagId == DefaultEnvironment.TestData.UrgentTagId);
    public Tag ImportantTag => Tags.First(t => t.TagId == DefaultEnvironment.TestData.ImportantTagId);
    public Tag BugTag => Tags.First(t => t.TagId == DefaultEnvironment.TestData.BugTagId);
    
    public Tasklist WorkList => Lists.First(l => l.ListId == DefaultEnvironment.TestData.WorkListId);
    public Tasklist PersonalList => Lists.First(l => l.ListId == DefaultEnvironment.TestData.PersonalListId);
    
    public Tickticktask FixLoginBugTask => Tasks.First(t => t.TaskId == DefaultEnvironment.TestData.WorkTask1Id);
    public Tickticktask ImplementSearchTask => Tasks.First(t => t.TaskId == DefaultEnvironment.TestData.WorkTask2Id);
}

/// <summary>
///     Seeds specific, easily referenceable test data with deterministic dates
/// </summary>
public class DefaultEnvironment(
    ILogger<EmptyEnvironment> logger,
    ISecurityService securityService,
    TimeProvider timeProvider,
    string? email = "test@user.dk",
    string? userId = "user-test-001") : ISeeder
{
    // Specific test data constants for easy reference in tests
    public static class TestData
    {
        public const string UserId = "user-test-001";
        public const string UserEmail = "test@user.dk";
        
        // List IDs and names
        public const string WorkListId = "list-work";
        public const string PersonalListId = "list-personal";
        public const string ShoppingListId = "list-shopping";
        
        // Tag IDs and names
        public const string UrgentTagId = "tag-urgent";
        public const string ImportantTagId = "tag-important";
        public const string BugTagId = "tag-bug";
        public const string FeatureTagId = "tag-feature";
        public const string TestingTagId = "tag-testing";
        
        // Task IDs
        public const string WorkTask1Id = "task-fix-login-bug";
        public const string WorkTask2Id = "task-implement-search";
        public const string WorkTask3Id = "task-update-docs";
        public const string PersonalTask1Id = "task-grocery-shopping";
        public const string PersonalTask2Id = "task-dentist-appointment";
        public const string ShoppingTask1Id = "task-buy-laptop";
        public const string ShoppingTask2Id = "task-buy-books";
        
        // Fixed test dates - now predictable!
        public static readonly DateTime BaseDate = new(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime UserCreatedAt = BaseDate.AddDays(-7);
        public static readonly DateTime TagsCreatedAt = BaseDate.AddDays(-6);
        public static readonly DateTime ListsCreatedAt = BaseDate.AddDays(-5);
        public static readonly DateTime Task1CreatedAt = BaseDate.AddDays(-3);
        public static readonly DateTime Task1DueDate = BaseDate.AddDays(1);
        public static readonly DateTime Task2CreatedAt = BaseDate.AddDays(-2);
        public static readonly DateTime Task2DueDate = BaseDate.AddDays(7);
        public static readonly DateTime Task3CreatedAt = BaseDate.AddDays(-4);
        public static readonly DateTime Task3DueDate = BaseDate.AddDays(3);
        public static readonly DateTime Task3CompletedAt = BaseDate.AddDays(-1);
    }

    public void CreateEnvironment(MyDbContext ctx)
    {
        var seededData = CreateEnvironmentWithReturn(ctx);
        // Original method just calls the new one and discards the return value
    }

    public SeededData CreateEnvironmentWithReturn(MyDbContext ctx)
    {
        ctx.Database.EnsureCreated();

        // Clear existing data
        ctx.TaskTags.RemoveRange(ctx.TaskTags);
        ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
        ctx.Tasklists.RemoveRange(ctx.Tasklists);
        ctx.Tags.RemoveRange(ctx.Tags);
        ctx.Users.RemoveRange(ctx.Users);

        var now = timeProvider.GetUtcNow().DateTime;

        // Create test user using constructor with fixed date
        var user = new User(
            email: TestData.UserEmail,
            salt: "6cbf8487-8f2c-48bc-a15c-33d88eae8b9e",
            passwordHash: "TJlSDc2mvpBmYKoi+2hnIOFx6ykf/V6JpmU7irhpoRcDT3KKUMwH7BWL/WlDTGrL11ud+5Q1BNxBEy3ZD1RRuQ==",
            role: Role.User,
            totpSecret: securityService.GenerateSecretKey(),
            createdAt: TestData.UserCreatedAt,
            userId: TestData.UserId
        );
        ctx.Users.Add(user);
        ctx.SaveChanges();

        // Create specific tags using constructors with fixed dates
        var tags = new List<Tag>
        {
            new Tag("urgent", TestData.UserId, TestData.UrgentTagId, TestData.TagsCreatedAt),
            new Tag("important", TestData.UserId, TestData.ImportantTagId, TestData.TagsCreatedAt),
            new Tag("bug", TestData.UserId, TestData.BugTagId, TestData.TagsCreatedAt),
            new Tag("feature", TestData.UserId, TestData.FeatureTagId, TestData.TagsCreatedAt),
            new Tag("testing", TestData.UserId, TestData.TestingTagId, TestData.TagsCreatedAt)
        };

        ctx.Tags.AddRange(tags);
        ctx.SaveChanges();

        // Create specific lists using constructors with fixed dates
        var lists = new List<Tasklist>
        {
            new Tasklist("Work", TestData.UserId, TestData.WorkListId, TestData.ListsCreatedAt),
            new Tasklist("Personal", TestData.UserId, TestData.PersonalListId, TestData.ListsCreatedAt),
            new Tasklist("Shopping", TestData.UserId, TestData.ShoppingListId, TestData.ListsCreatedAt)
        };

        ctx.Tasklists.AddRange(lists);
        ctx.SaveChanges();

        // Create specific tasks using constructors with fixed dates
        var tasks = new List<Tickticktask>
        {
            new Tickticktask(TestData.WorkListId, "Fix login authentication bug", 
                "Users are unable to login with valid credentials. Critical bug affecting production.",
                TestData.Task1DueDate, 5, false, null, TestData.Task1CreatedAt, TestData.WorkTask1Id),
            
            new Tickticktask(TestData.WorkListId, "Implement search functionality",
                "Add full-text search capability to the main dashboard with filters and sorting.",
                TestData.Task2DueDate, 3, false, null, TestData.Task2CreatedAt, TestData.WorkTask2Id),
            
            new Tickticktask(TestData.WorkListId, "Update API documentation",
                "Update OpenAPI documentation to reflect recent endpoint changes.",
                TestData.Task3DueDate, 2, true, TestData.Task3CompletedAt, TestData.Task3CreatedAt, TestData.WorkTask3Id),
            
            new Tickticktask(TestData.PersonalListId, "Grocery shopping",
                "Buy weekly groceries: milk, bread, eggs, vegetables, and fruits.",
                TestData.BaseDate.AddDays(2), 2, false, null, TestData.BaseDate.AddDays(-1), TestData.PersonalTask1Id),
            
            new Tickticktask(TestData.PersonalListId, "Schedule dentist appointment",
                "Call dentist office to schedule routine cleaning and checkup.",
                TestData.BaseDate.AddDays(5), 1, true, TestData.BaseDate.AddHours(-2), TestData.BaseDate.AddDays(-2), TestData.PersonalTask2Id),
            
            new Tickticktask(TestData.ShoppingListId, "Buy new laptop",
                "Research and purchase a new development laptop with 32GB RAM and SSD.",
                TestData.BaseDate.AddDays(14), 4, false, null, TestData.BaseDate.AddDays(-3), TestData.ShoppingTask1Id),
            
            new Tickticktask(TestData.ShoppingListId, "Buy programming books",
                "Purchase books on clean architecture and design patterns.",
                TestData.BaseDate.AddDays(10), 1, false, null, TestData.BaseDate.AddDays(-1), TestData.ShoppingTask2Id)
        };

        ctx.Tickticktasks.AddRange(tasks);
        ctx.SaveChanges();

        // Create specific task-tag relationships using constructors with fixed dates
        var taskTags = new List<TaskTag>
        {
            new TaskTag(TestData.WorkTask1Id, TestData.UrgentTagId, TestData.Task1CreatedAt),
            new TaskTag(TestData.WorkTask1Id, TestData.BugTagId, TestData.Task1CreatedAt),
            new TaskTag(TestData.WorkTask2Id, TestData.FeatureTagId, TestData.Task2CreatedAt),
            new TaskTag(TestData.WorkTask2Id, TestData.ImportantTagId, TestData.Task2CreatedAt),
            new TaskTag(TestData.WorkTask3Id, TestData.TestingTagId, TestData.Task3CreatedAt),
            new TaskTag(TestData.PersonalTask1Id, TestData.ImportantTagId, TestData.BaseDate.AddDays(-1)),
            new TaskTag(TestData.PersonalTask2Id, TestData.UrgentTagId, TestData.BaseDate.AddDays(-2)),
            new TaskTag(TestData.ShoppingTask1Id, TestData.ImportantTagId, TestData.BaseDate.AddDays(-3)),
            new TaskTag(TestData.ShoppingTask2Id, TestData.FeatureTagId, TestData.BaseDate.AddDays(-1))
        };

        ctx.TaskTags.AddRange(taskTags);
        ctx.SaveChanges();

        logger.LogInformation("DefaultEnvironment seeded successfully with deterministic test data");

        return new SeededData
        {
            User = user,
            Tags = tags,
            Lists = lists,
            Tasks = tasks,
            TaskTags = taskTags
        };
    }
}
