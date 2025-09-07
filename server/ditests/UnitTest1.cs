// using System.Text.Json;
// using api.Etc;
// using api.Models;
// using api.Models.Dtos.Requests;
// using api.Services;
//
// namespace ditests
// {
//     public class UnitTest1(ITaskService taskService, 
//         ISeeder seeder,
//         ITestOutputHelper testOutputHelper,
//         ITestDataIds ids)
//     {
//         [Fact]
//         public async Task GetLists_ShouldReturn3ListsForJohn_WithTestSeeder()
//         {
//             seeder.SeedDatabase();
//             var result = await taskService.GetMyLists(new JwtClaims() { Id = ids.JohnId });
//             Assert.Equal(3, result.Count);
//         }
//         
//         [Fact]
//         public async Task GetTasks()
//         {
//             seeder.SeedDatabase();
//             var result = await taskService.GetMyTasks(new TaskFilteringAndSorting
//             {
//                 EarliestDueDate = DateTime.UtcNow.Date.AddDays(-1),
//                 IsDescending = true
//             },new JwtClaims() { Id = ids.JohnId });
//             Assert.Equal(3, result.Count);
//         }
//     }
// }
//
//
//
