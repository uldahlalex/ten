using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Etc;

/// <summary>
/// Simple singleton service that holds test data IDs for consistent test data
/// </summary>
public interface ITestDataIds
{
    // User IDs
    string JohnId { get; }
    string JaneId { get; }
    string AdminId { get; }
    
    // Tag IDs
    string UrgentTagId { get; }
    string BugTagId { get; }
    string FeatureTagId { get; }
    string ImportantTagId { get; }
    string PersonalTagId { get; }
    
    // Tasklist IDs
    string WorkListId { get; }
    string PersonalListId { get; }
    string JanePersonalListId { get; }
    string ShoppingListId { get; }
    
    // Task IDs
    string CriticalBugTaskId { get; }
    string SearchFeatureTaskId { get; }
    string UpdateDocsTaskId { get; }
    string GroceriesTaskId { get; }
    string DentistTaskId { get; }
}

/// <summary>
/// Implementation with hardcoded test IDs
/// </summary>
public class TestDataIds : ITestDataIds
{
    // User IDs
    public string JohnId => "user-1";
    public string JaneId => "jane-002";
    public string AdminId => "admin-003";
    
    // Tag IDs
    public string UrgentTagId => "tag-urgent";
    public string BugTagId => "tag-bug";
    public string FeatureTagId => "tag-feature";
    public string ImportantTagId => "tag-important";
    public string PersonalTagId => "tag-personal";
    
    // Tasklist IDs
    public string WorkListId => "list-work";
    public string PersonalListId => "list-personal";
    public string JanePersonalListId => "list-jane-personal";
    public string ShoppingListId => "list-shopping";
    
    // Task IDs
    public string CriticalBugTaskId => "task-critical-bug";
    public string SearchFeatureTaskId => "task-search";
    public string UpdateDocsTaskId => "task-docs";
    public string GroceriesTaskId => "task-groceries";
    public string DentistTaskId => "task-dentist";
}

/// <summary>
/// Test database seeder that creates objects using the test IDs
/// </summary>
public class TestDataSeeder(ITestDataIds ids,
    ITotpService totpService, TimeProvider timeProvider, ILogger<TestDataSeeder> logger, MyDbContext ctx) : ISeeder
{
    private readonly DateTime _baseTime = timeProvider.GetUtcNow().UtcDateTime;

    public void SeedDatabase()
    {
        try
        {
            ctx.Database.EnsureCreated();
            
            ClearDatabase(ctx);

            // Create users
            var john = new User(
                email: "john@example.com",
                salt: "f50528c0-0292-4e71-a2b3-bc6cd41ab884",
                passwordHash: "6z29FedE+NmCXtu/ANEyxMBKys5F9yAoh3AKgRMosei+Q+nJowkSoaV/RFxzpsRMnp1L1l+tVnH5qTx3Mgc4tA==",
                role: Role.User,
                totpSecret: totpService.GenerateSecretKey(),
                createdAt: _baseTime.AddDays(-30),
                userId: ids.JohnId
            );

            var jane = new User(
                email: "jane@example.com",
                salt: "f50528c0-0292-4e71-a2b3-bc6cd41ab884",
                passwordHash: "6z29FedE+NmCXtu/ANEyxMBKys5F9yAoh3AKgRMosei+Q+nJowkSoaV/RFxzpsRMnp1L1l+tVnH5qTx3Mgc4tA==",
                role: Role.User,
                totpSecret: totpService.GenerateSecretKey(),
                createdAt: _baseTime.AddDays(-25),
                userId: ids.JaneId
            );

            var admin = new User(
                email: "admin@example.com",
                salt: "f50528c0-0292-4e71-a2b3-bc6cd41ab884",
                passwordHash: "6z29FedE+NmCXtu/ANEyxMBKys5F9yAoh3AKgRMosei+Q+nJowkSoaV/RFxzpsRMnp1L1l+tVnH5qTx3Mgc4tA==",
                role: Role.User,
                totpSecret: totpService.GenerateSecretKey(),
                createdAt: _baseTime.AddDays(-35),
                userId: ids.AdminId
            );

            var users = new[] { john, jane, admin };
            ctx.Users.AddRange(users);
            ctx.SaveChanges();

            // Create tags
            var urgentTag = new Tag(_baseTime.AddDays(-25), "Urgent", john.UserId, ids.UrgentTagId);
            var bugTag = new Tag(_baseTime.AddDays(-25), "Bug", john.UserId, ids.BugTagId);
            var featureTag = new Tag(_baseTime.AddDays(-25), "Feature", john.UserId, ids.FeatureTagId);
            var importantTag = new Tag(_baseTime.AddDays(-25), "Important", john.UserId, ids.ImportantTagId);
            var personalTag = new Tag(_baseTime.AddDays(-20), "Personal", jane.UserId, ids.PersonalTagId);

            var tags = new[] { urgentTag, bugTag, featureTag, importantTag, personalTag };
            ctx.Tags.AddRange(tags);
            ctx.SaveChanges();

            // Create task lists
            var workList = new Tasklist(_baseTime.AddDays(-20), "Work Tasks", john.UserId, ids.WorkListId);
            var personalList = new Tasklist(_baseTime.AddDays(-20), "Personal Tasks", john.UserId, ids.PersonalListId);
            var janePersonalList = new Tasklist(_baseTime.AddDays(-18), "Jane's Tasks", jane.UserId, ids.JanePersonalListId);
            var shoppingList = new Tasklist(_baseTime.AddDays(-15), "Shopping", john.UserId, ids.ShoppingListId);

            var tasklists = new[] { workList, personalList, janePersonalList, shoppingList };
            ctx.Tasklists.AddRange(tasklists);
            ctx.SaveChanges();

            // Create tasks
            var criticalBugTask = new Tickticktask(
                createdAt: _baseTime.AddDays(-5),
                listId: workList.ListId,
                title: "Fix critical login bug",
                description: "Users cannot log in, blocking all functionality",
                dueDate: _baseTime.AddDays(1),
                priority: 5,
                completed: false,
                completedAt: null,
                taskId: ids.CriticalBugTaskId
            );

            var searchFeatureTask = new Tickticktask(
                createdAt: _baseTime.AddDays(-3),
                listId: workList.ListId,
                title: "Implement search feature",
                description: "Add full-text search to dashboard",
                dueDate: _baseTime.AddDays(7),
                priority: 3,
                completed: false,
                completedAt: null,
                taskId: ids.SearchFeatureTaskId
            );

            var updateDocsTask = new Tickticktask(
                createdAt: _baseTime.AddDays(-4),
                listId: workList.ListId,
                title: "Update API documentation",
                description: "Update OpenAPI specs for new endpoints",
                dueDate: _baseTime.AddDays(3),
                priority: 2,
                completed: true,
                completedAt: _baseTime.AddDays(-1),
                taskId: ids.UpdateDocsTaskId
            );

            var groceriesTask = new Tickticktask(
                createdAt: _baseTime.AddDays(-1),
                listId: personalList.ListId,
                title: "Buy groceries",
                description: "Weekly shopping: milk, bread, eggs",
                dueDate: _baseTime.AddDays(2),
                priority: 2,
                completed: false,
                completedAt: null,
                taskId: ids.GroceriesTaskId
            );

            var dentistTask = new Tickticktask(
                createdAt: _baseTime.AddDays(-2),
                listId: janePersonalList.ListId,
                title: "Schedule dentist appointment",
                description: "Annual checkup and cleaning",
                dueDate: _baseTime.AddDays(5),
                priority: 1,
                completed: false,
                completedAt: null,
                taskId: ids.DentistTaskId
            );

            var tasks = new[] { criticalBugTask, searchFeatureTask, updateDocsTask, groceriesTask, dentistTask };
            ctx.Tickticktasks.AddRange(tasks);
            ctx.SaveChanges();

            // Create task-tag relationships
            var taskTagPairs = new[]
            {
                (criticalBugTask, urgentTag),
                (criticalBugTask, bugTag),
                (searchFeatureTask, featureTag),
                (searchFeatureTask, importantTag),
                (groceriesTask, importantTag),
                (dentistTask, personalTag)
            };

            var taskTags = taskTagPairs.Select(pair => 
                new TaskTag(
                    createdAt: _baseTime.AddDays(-3),
                    taskId: pair.Item1.TaskId,
                    tagId: pair.Item2.TagId
                )).ToList();
                
            ctx.TaskTags.AddRange(taskTags);
            ctx.SaveChanges();

            logger.LogInformation("Test environment seeded with {UserCount} users, {TagCount} tags, {TaskCount} tasks",
                users.Length, tags.Length, tasks.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database - this may be due to concurrent test execution");
            // Don't rethrow - let tests continue with potentially empty database
        }
    }

    private void ClearDatabase(MyDbContext ctx)
    {
       
            // Clear in dependency order to avoid foreign key constraint violations
            if (ctx.TaskTags.Any()) ctx.TaskTags.RemoveRange(ctx.TaskTags);
            if (ctx.Tickticktasks.Any()) ctx.Tickticktasks.RemoveRange(ctx.Tickticktasks);
            if (ctx.Tasklists.Any()) ctx.Tasklists.RemoveRange(ctx.Tasklists);
            if (ctx.Tags.Any()) ctx.Tags.RemoveRange(ctx.Tags);
            if (ctx.Users.Any()) ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
     
    }
}

public interface ISeeder
{
    public void SeedDatabase();
}

