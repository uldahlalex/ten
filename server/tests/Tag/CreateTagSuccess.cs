using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using tests;

public class CreateTagSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public async System.Threading.Tasks.Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder);
        builder.DefaultTestConfig();

        _app = builder.Build();
        Program.ConfigureApp(_app);
        await _app.StartAsync();

        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6InVzZXItMSJ9.LUnCy-TvtvyRhFyyg2qFFwhGMLYAFFFqrKEcBLFAf1Q");
    }


    [Test]
    public async System.Threading.Tasks.Task CreateTag()
    {
        var tag = new CreateTagRequestDto()
        {
           TagName = "My new tag"
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateTag), tag);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Response status code expected OK but got "+response.StatusCode+ " with message: "+await response.Content.ReadAsStringAsync());
        
        
        var result = await response.Content.ReadFromJsonAsync<TagDto>() ?? throw new Exception("Response content is null");
        Validator.ValidateObject(result, new ValidationContext(result), true);
        if(result.Name!=tag.TagName)
            throw new Exception("Expected tag name to be "+tag.TagName+" but got: "+result.Name);
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>()
            .Tags.First(t => t.TagId == result.TagId); //throws or finds
        
    }
    
}