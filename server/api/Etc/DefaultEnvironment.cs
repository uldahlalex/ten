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
public class DefaultEnvironment : ISeeder
{
    private readonly ILogger<DefaultEnvironment> _logger;
    private readonly ISecurityService _securityService;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Seeds specific, easily referenceable test data with deterministic dates
    /// </summary>
    public DefaultEnvironment(ILogger<DefaultEnvironment> logger,
        ISecurityService securityService,
        TimeProvider timeProvider)
    {
        _logger = logger;
        _securityService = securityService;
        _timeProvider = timeProvider;
    }

    // Specific test data constants for easy reference in tests
    public class TestData
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
    }

    // Helper method to get deterministic dates based on TimeProvider
    private DateTime GetBaseDate() => _timeProvider.GetUtcNow().DateTime;
    private DateTime GetUserCreatedAt() => GetBaseDate().AddDays(-7);
    private DateTime GetTagsCreatedAt() => GetBaseDate().AddDays(-6);
    private DateTime GetListsCreatedAt() => GetBaseDate().AddDays(-5);
    private DateTime GetTask1CreatedAt() => GetBaseDate().AddDays(-3);
    private DateTime GetTask1DueDate() => GetBaseDate().AddDays(1);
    private DateTime GetTask2CreatedAt() => GetBaseDate().AddDays(-2);
    private DateTime GetTask2DueDate() => GetBaseDate().AddDays(7);
    private DateTime GetTask3CreatedAt() => GetBaseDate().AddDays(-4);
    private DateTime GetTask3DueDate() => GetBaseDate().AddDays(3);
    private DateTime GetTask3CompletedAt() => GetBaseDate().AddDays(-1);

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

        // Create test user using constructor with TimeProvider-based date
        var user = new User(
            email: TestData.UserEmail,
            salt: "6cbf8487-8f2c-48bc-a15c-33d88eae8b9e",
            passwordHash: "TJlSDc2mvpBmYKoi+2hnIOFx6ykf/V6JpmU7irhpoRcDT3KKUMwH7BWL/WlDTGrL11ud+5Q1BNxBEy3ZD1RRuQ==",
            role: Role.User,
            totpSecret: _securityService.GenerateSecretKey(),
            createdAt: GetUserCreatedAt(),
            userId: TestData.UserId
        );
        ctx.Users.Add(user);
        ctx.SaveChanges();

        // Create specific tags using constructors with fixed dates (createdAt first)
        var tags = new List<Tag>
        {
            new Tag(GetTagsCreatedAt(), "urgent", TestData.UserId, TestData.UrgentTagId),
            new Tag(GetTagsCreatedAt(), "important", TestData.UserId, TestData.ImportantTagId),
            new Tag(GetTagsCreatedAt(), "bug", TestData.UserId, TestData.BugTagId),
            new Tag(GetTagsCreatedAt(), "feature", TestData.UserId, TestData.FeatureTagId),
            new Tag(GetTagsCreatedAt(), "testing", TestData.UserId, TestData.TestingTagId)
        };

        ctx.Tags.AddRange(tags);
        ctx.SaveChanges();

        // Create specific lists using constructors with fixed dates
        var lists = new List<Tasklist>
        {
            new Tasklist(GetListsCreatedAt(), "Work", TestData.UserId, TestData.WorkListId),
            new Tasklist(GetListsCreatedAt(), "Personal", TestData.UserId, TestData.PersonalListId),
            new Tasklist(GetListsCreatedAt(), "Shopping", TestData.UserId, TestData.ShoppingListId)
        };

        ctx.Tasklists.AddRange(lists);
        ctx.SaveChanges();

        // Create specific tasks using constructors with fixed dates
        var tasks = new List<Tickticktask>
        {
            new Tickticktask(GetTask1CreatedAt(), TestData.WorkListId, "Fix login authentication bug", 
                "Users are unable to login with valid credentials. Critical bug affecting production.",
                GetTask1DueDate(), 5, false, null, TestData.WorkTask1Id),
            
            new Tickticktask(GetTask2CreatedAt(), TestData.WorkListId, "Implement search functionality",
                "Add full-text search capability to the main dashboard with filters and sorting.",
                GetTask2DueDate(), 3, false, null, TestData.WorkTask2Id),
            
            new Tickticktask(GetTask3CreatedAt(), TestData.WorkListId, "Update API documentation",
                "Update OpenAPI documentation to reflect recent endpoint changes.",
                GetTask3DueDate(), 2, true, GetTask3CompletedAt(), TestData.WorkTask3Id),
            
            new Tickticktask(GetBaseDate().AddDays(-1), TestData.PersonalListId, "Grocery shopping",
                "Buy weekly groceries: milk, bread, eggs, vegetables, and fruits.",
                GetBaseDate().AddDays(2), 2, false, null, TestData.PersonalTask1Id),
            
            new Tickticktask(GetBaseDate().AddDays(-2), TestData.PersonalListId, "Schedule dentist appointment",
                "Call dentist office to schedule routine cleaning and checkup.",
                GetBaseDate().AddDays(5), 1, true, GetBaseDate().AddHours(-2), TestData.PersonalTask2Id),
            
            new Tickticktask(GetBaseDate().AddDays(-3), TestData.ShoppingListId, "Buy new laptop",
                "Research and purchase a new development laptop with 32GB RAM and SSD.",
                GetBaseDate().AddDays(14), 4, false, null, TestData.ShoppingTask1Id),
            
            new Tickticktask(GetBaseDate().AddDays(-1), TestData.ShoppingListId, "Buy programming books",
                "Purchase books on clean architecture and design patterns.",
                GetBaseDate().AddDays(10), 1, false, null, TestData.ShoppingTask2Id)
        };

        ctx.Tickticktasks.AddRange(tasks);
        ctx.SaveChanges();

        // Create specific task-tag relationships using constructors with fixed dates (createdAt first)
        var taskTags = new List<TaskTag>
        {
            new TaskTag(GetTask1CreatedAt(), TestData.WorkTask1Id, TestData.UrgentTagId),
            new TaskTag(GetTask1CreatedAt(), TestData.WorkTask1Id, TestData.BugTagId),
            new TaskTag(GetTask2CreatedAt(), TestData.WorkTask2Id, TestData.FeatureTagId),
            new TaskTag(GetTask2CreatedAt(), TestData.WorkTask2Id, TestData.ImportantTagId),
            new TaskTag(GetTask3CreatedAt(), TestData.WorkTask3Id, TestData.TestingTagId),
            new TaskTag(GetBaseDate().AddDays(-1), TestData.PersonalTask1Id, TestData.ImportantTagId),
            new TaskTag(GetBaseDate().AddDays(-2), TestData.PersonalTask2Id, TestData.UrgentTagId),
            new TaskTag(GetBaseDate().AddDays(-3), TestData.ShoppingTask1Id, TestData.ImportantTagId),
            new TaskTag(GetBaseDate().AddDays(-3), TestData.ShoppingTask2Id, TestData.FeatureTagId)
        };

        ctx.TaskTags.AddRange(taskTags);
        ctx.SaveChanges();

        _logger.LogInformation("DefaultEnvironment seeded successfully with deterministic test data");

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