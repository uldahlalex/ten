using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{
    public const string GetTasksRoute = nameof(GetTasks);

    public const string CreateTaskRoute = nameof(CreateTask);

    [HttpGet]
    [Route(GetTasksRoute)]
    public ActionResult<List<TickticktaskDto>> GetTasks([FromHeader] string authorization)
    {
        _ = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Tickticktasks.Select(task => task.ToDto()).ToList();
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