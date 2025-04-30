using System.Text.Json;
using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using LightQuery;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, 
    ISecurityService securityService, ILogger<TicktickTaskController> logger) : Controller
{
    public const string GetMyTasksRoute = nameof(GetMyTasks);
    [HttpPost]
    [LightQuery]
    [Route(GetMyTasksRoute)]
    public async Task<ActionResult<TickticktaskDto>> GetMyTasks(
        
        [FromHeader]string authorization)
    {
        var userId = securityService.VerifyJwtOrThrow(authorization).Id;
        var tasks = ctx.Tickticktasks;


        var dtos = tasks
            .Select(task => task.ToDto());
         logger.LogInformation(JsonSerializer.Serialize(dtos));
        
        return Ok(dtos);
    }
    
    public const string GetTasksRoute = nameof(GetTasks);

    public const string CreateTaskRoute = nameof(CreateTask);

    [HttpGet]
    [Route(GetTasksRoute)]
    public ActionResult<List<TickticktaskDto>> GetTasks([FromHeader] string authorization)
    {
        _ = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Tickticktasks
            //here i just do DbSet.ApplyFiltering() where the input object comes from the http request maybe
            .Select(task => task.ToDto()).ToList();
        return Ok(result);
    }

    [HttpPost]
    [Route(CreateTaskRoute)]
    public ActionResult<TickticktaskDto> CreateTask([FromBody] CreateTaskRequestDto dto,
        [FromHeader] string authorization)
    {
        _ = securityService.VerifyJwtOrThrow(authorization);
        
        if(dto.DueDate < DateTime.UtcNow)
            return BadRequest("Due date cannot be in the past");
        
        var list = ctx.Tasklists.First(list => list.ListId == dto.ListId);
        var tags = dto.TaskTagsDtos.Select(taskTagDto =>
            ctx.TaskTags.First(tag => tag.TagId == taskTagDto.TagId)).ToList();

        var entity = dto.ToEntity(tags, list);

        ctx.Tickticktasks.Add(entity);
        ctx.SaveChanges();

        return Ok(entity.ToDto());
    }


}