using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Generated;

namespace tests.Tag;

public class CreateTagSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;
    private IApiClient _apiClient = null!;

    [Before(Test)]
    public Task Setup()
    {
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting();
        builder.AddProgramcsServices();
        builder.ModifyServicesForTesting();
        _app = builder.Build();

        _app.BeforeProgramcsMiddleware();
        _app.AddProgramcsMiddleware();
        _app.AfterProgramcsMiddleware();

        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        _apiClient = new ApiClient(_baseUrl, _client);
        return Task.CompletedTask;
    }

    [Test]
    public async Task CreateTag()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var timeProvider = _scopedServiceProvider.GetRequiredService<TimeProvider>();
        
        var newTagName = "My new tag";
        var tag = new CreateTagRequestDto(newTagName);

        var result = await _apiClient.TicktickTask_CreateTagAsync(tag);
            
        Validator.ValidateObject(result, new ValidationContext(result), true);
        
        if (result.Name != newTagName)
            throw new Exception($"Expected tag name to be '{newTagName}' but got '{result.Name}'");
            
        if (string.IsNullOrEmpty(result.TagId))
            throw new Exception("Created tag should have a non-empty TagId");
            
        // Verify tag exists in database
        var dbTag = ctx.Tags.FirstOrDefault(t => t.TagId == result.TagId);
        if (dbTag == null)
            throw new Exception($"Tag with ID {result.TagId} should exist in database after creation");
            
        if (dbTag.Name != newTagName)
            throw new Exception($"Database tag name should be '{newTagName}' but got '{dbTag.Name}'");
            
        // Verify CreatedAt is recent
        if (Math.Abs((timeProvider.GetUtcNow().UtcDateTime - result.CreatedAt).TotalSeconds) > 2)
            throw new Exception("CreatedAt timestamp should be within 2 seconds of now");
    }
}