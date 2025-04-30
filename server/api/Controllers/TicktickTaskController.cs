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

    
    public const string GetTasksRoute = nameof(GetTasks);

    public const string CreateTaskRoute = nameof(CreateTask);

    [HttpPost]
    [Route(GetTasksRoute)]
    public ActionResult<List<TickticktaskDto>> GetTasks(
        [FromHeader] string authorization,
        [FromBody]GetTasksFilterAndOrderParameters parameters)
    {
        var userClaims = securityService.VerifyJwtOrThrow(authorization);
        var baseQuery = ctx.Tickticktasks;
    
        var specification = new TaskSpecification(parameters);
        var result = baseQuery
                .ApplySpecification(specification)
                .Select(task => task.ToDto())
                .ToList();
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