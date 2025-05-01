using System.Text;
using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api;

public class TaskService(ISecurityService securityService, MyDbContext ctx, ILogger<TaskService> logger) : ITaskService
{
    public async Task<List<TickticktaskDto>> GetTasks(GetTasksFilterAndOrderParameters parameters, JwtClaims jwtClaims)
    {
        IQueryable<Tickticktask> query = ctx.Tickticktasks;
        if (parameters.ListIds != null && parameters.ListIds?.Count > 0)
        {
            //Logical OR inclusion
           query = query.Where(task => parameters.ListIds.Contains(task.ListId));
        }

        if (parameters.MinPriority != null)
        {
            query = query.Where(t => t.Priority > parameters.MinPriority);
        }

        if (parameters.MaxPriority != null)
        {
            query = query.Where(t => t.Priority < parameters.MaxPriority);
        }

        if (parameters.IsCompleted != null)
        {
            query = query.Where(t => t.Completed == parameters.IsCompleted);
        }

        if (parameters.LatestDueDate != null)
        {
            query = query.Where(t => t.DueDate < parameters.LatestDueDate);
        }

        if (parameters.EarliestDueDate !=null)
        {
            query = query.Where(t => t.DueDate > parameters.EarliestDueDate);
        }

        if (parameters.TagIds != null && parameters.TagIds.Count > 0)
        {
            //logical OR inclusion
            query = query
                .Where(t => parameters.TagIds
                    .Any(f => t.TaskTags.Select(tag => tag.TagId)
                    .Contains(f))
                );
        }
        
        
        if (parameters.SearchTerm != null)
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(searchTerm) || t.Description.ToLower().Contains(searchTerm));
        }

        if (parameters.OrderBy != null)
        {
            if (parameters.OrderBy == nameof(Tickticktask.Priority))
                query = query.OrderBy(t => t.Priority);
            if(parameters.OrderBy == nameof(Tickticktask.DueDate))
                query = query.OrderBy(t => t.DueDate);
        }
        if (parameters.IsDescending != null)
        {
            query = query.Reverse();
        }

        return query.Select(t => t.ToDto()).ToList();
    }

    public async Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, JwtClaims jwtClaims)
    {

        if (dto.DueDate < DateTime.UtcNow)
            throw new Exception("Due date cannot be in the past");
        
        var list = ctx.Tasklists.First(list => list.ListId == dto.ListId);
        var tags = dto.TaskTagsDtos.Select(taskTagDto =>
            ctx.TaskTags.First(tag => tag.TagId == taskTagDto.TagId)).ToList();

        var entity = dto.ToEntity(tags, list);

        ctx.Tickticktasks.Add(entity);
        ctx.SaveChanges();
        return entity.ToDto();

    }

    public Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims)
    {
        throw new NotImplementedException();
    }
}

public interface ITaskService
{

    public Task<List<TickticktaskDto>> GetTasks(GetTasksFilterAndOrderParameters parameters, JwtClaims jwtClaims);
    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, JwtClaims claims);
    Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims);
}