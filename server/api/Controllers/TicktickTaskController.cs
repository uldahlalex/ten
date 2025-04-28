using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{
    public const string GetMyTasksRoute = nameof(GetMyTasks);
    [HttpGet]
    [Route(GetMyTasksRoute)]
    public async Task<ActionResult<List<TickticktaskDto>>> GetMyTasks(
        
        [FromHeader]string authorization,     
            [FromQuery] TaskQueryParams queryParams)
    {
        var userId = securityService.VerifyJwtOrThrow(authorization).Id;
        IQueryable<Tickticktask> query = ctx.Tickticktasks
            .Where(t => t.List.UserId == userId); 
    
        if (queryParams.ListIds?.Any() == true)
        {
            query = query.Where(t => queryParams.ListIds.Contains(t.ListId));
        }
    if (queryParams.ListIds?.Any() == true)
    {
        query = query.Where(t => queryParams.ListIds.Contains(t.ListId));
    }

    if (queryParams.IsCompleted.HasValue)
    {
        query = query.Where(t => t.Completed == queryParams.IsCompleted.Value);
    }

    if (queryParams.DueDateStart.HasValue)
    {
        query = query.Where(t => t.DueDate >= queryParams.DueDateStart.Value);
    }

    if (queryParams.DueDateEnd.HasValue)
    {
        query = query.Where(t => t.DueDate <= queryParams.DueDateEnd.Value);
    }

    if (queryParams.MinPriority.HasValue)
    {
        query = query.Where(t => t.Priority >= queryParams.MinPriority.Value);
    }

    if (queryParams.MaxPriority.HasValue)
    {
        query = query.Where(t => t.Priority <= queryParams.MaxPriority.Value);
    }

    if (!string.IsNullOrEmpty(queryParams.SearchTerm))
    {
        var searchTerm = queryParams.SearchTerm.ToLower();
        query = query.Where(t => 
            t.Title.ToLower().Contains(searchTerm) || 
            t.Description.ToLower().Contains(searchTerm));
    }

    if (queryParams.TagIds?.Any() == true)
    {
        query = query.Where(t => 
            t.TaskTags.Any(tt => queryParams.TagIds.Contains(tt.TagId)));
    }

    // Apply ordering
    query = queryParams.OrderBy?.Value switch
    {
        string when TaskOrderBy.DueDate == TaskOrderBy.DueDate.Value => 
            queryParams.IsDescending 
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
        string when TaskOrderBy.Priority == TaskOrderBy.Priority.Value => 
            queryParams.IsDescending 
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
        string when TaskOrderBy.CreatedAt == TaskOrderBy.CreatedAt.Value => 
            queryParams.IsDescending 
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
        string when TaskOrderBy.CompletedAt == TaskOrderBy.CompletedAt.Value => 
            queryParams.IsDescending 
                ? query.OrderByDescending(t => t.CompletedAt)
                : query.OrderBy(t => t.CompletedAt),
        _ => query.OrderByDescending(t => t.CreatedAt) 
    };
            
         var dtos =   query
            .Select(task => task.ToDto())
            .ToList();
        
        return Ok(dtos);
    }
    
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