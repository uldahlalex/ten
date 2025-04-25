using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{

    public const string GetTasksRoute = nameof(GetTasks);
    
    [HttpGet]
    [Route(nameof(GetTasksRoute))]
    public ActionResult<List<TickticktaskDto>> GetTasks([FromHeader]string authorization)
    {
        var requester = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Tickticktasks.Select(task => new TickticktaskDto().FromEntity(task)).ToList();
        return Ok(result);
    }

    public const string CreateTaskRoute = nameof(CreateTask);
    [HttpPost]
    [Route(nameof(CreateTaskRoute))]
    public async Task<ActionResult> CreateTask([FromBody]CreateTaskRequestDto dto)
    {
        var list = ctx.Tasklists.First(list => list.ListId == dto.ListId);
        var tags = dto.TaskTagsDtos.Select(taskTagDto => 
            (ctx.TaskTags.First(tag => tag.TagId == taskTagDto.TagId))).ToList();

        var entity = dto.ToEntity(tags, list);

        ctx.Tickticktasks.Add(entity);
        ctx.SaveChanges();
        
        return Ok(entity);
    }
    
    
}