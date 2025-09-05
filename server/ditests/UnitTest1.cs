using System.Text.Json;
using api.Etc;
using api.Models;
using api.Services;

namespace ditests
{
    public class UnitTest1(ITaskService taskService, 
        ISeeder seeder,
        ITestDataIds ids)
    {
        [Fact]
        public async Task Test1()
        {
            seeder.SeedDatabase();
            var result = await taskService.GetMyLists(new JwtClaims() { Id = ids.JohnId });
            JsonSerializer.Serialize(result);
        }
    }
}



