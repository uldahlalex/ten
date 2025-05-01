using System.ComponentModel.DataAnnotations;
using api.Mappers;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api;

public class TaskService(ISecurityService securityService, MyDbContext ctx, ILogger<TaskService> logger) : ITaskService
{
    public async Task<List<TickticktaskDto>> GetMyTasks(GetTasksFilterAndOrderParameters parameters, JwtClaims jwtClaims)
    {
        IQueryable<Tickticktask> query = ctx.Tickticktasks;
        if (parameters.ListIds != null && parameters.ListIds?.Count > 0)
        {
            //Logical OR inclusion (list belongs to user, so no need to check for user)
           query = query.Where(task => parameters.ListIds.Contains(task.ListId));
        }

        if (parameters.MinPriority != null)
        {
            query = query.Where(t => t.Priority >= parameters.MinPriority);
        }

        if (parameters.MaxPriority != null)
        {
            query = query.Where(t => t.Priority <= parameters.MaxPriority);
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
            throw new ValidationException("Due date cannot be in the past");
        
        var list = ctx.Tasklists.First(list => list.ListId == dto.ListId);
        var tags = dto.TaskTagsDtos.Select(taskTagDto =>
            ctx.TaskTags.First(tag => tag.TagId == taskTagDto.TagId)).ToList();

        var entity = new Tickticktask
            {
                TaskId = Guid.NewGuid().ToString(),
                ListId = dto.ListId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                CreatedAt = DateTime.UtcNow,
                Completed = false,
                CompletedAt = null,
                List = list,
                TaskTags = tags
            };

        ctx.Tickticktasks.Add(entity);
        ctx.SaveChanges();
        return entity.ToDto();

    }

    public async Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims)
    {
        if (dto.DueDate < DateTime.UtcNow)
            throw new ValidationException("Due date cannot be in the past");

        var existing = ctx.Tickticktasks.First(t => t.TaskId == dto.Id);
        
        var newList = ctx.Tasklists.First(list => list.ListId == dto.ListId);
        var newTags = dto.TaskTagsDtos.Select(taskTagDto =>
            ctx.TaskTags.First(tag => tag.TagId == taskTagDto.TagId)).ToList();
        
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.DueDate = dto.DueDate;
        existing.Priority = dto.Priority;
        existing.List = newList;
        existing.TaskTags = newTags;
        existing.Completed = dto.Completed;
        existing.CompletedAt = dto.Completed ? DateTime.UtcNow : null;
        
        
        ctx.Tickticktasks.Update(existing);
        ctx.SaveChanges();
        return existing.ToDto();
    }

    public Task DeleteTask(string taskId, JwtClaims claims)
    {
        var task = ctx.Tickticktasks.First(t => t.TaskId == taskId);
        ctx.Tickticktasks.Remove(task);
        ctx.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<List<TasklistDto>> GetMyLists(JwtClaims claims)
    {
        var lists = ctx.Tasklists
            .Where(list => list.UserId == claims.Id)
            .Select(list => list.ToDto())
            .ToList();
        return Task.FromResult(lists);
    }

    public Task<List<TagDto>> GetMyTags(JwtClaims claims)
    {
        var tags = ctx.Tags
            .Where(tag => tag.UserId == claims.Id)
            .Select(tag => tag.ToDto())
            .ToList();
        return Task.FromResult(tags);
    }
}

public interface ITaskService
{

    public Task<List<TickticktaskDto>> GetMyTasks(GetTasksFilterAndOrderParameters parameters, JwtClaims jwtClaims);
    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, JwtClaims claims);
    Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims);
    Task DeleteTask(string taskId, JwtClaims claims);
    Task<List<TasklistDto>> GetMyLists(JwtClaims claims);
    Task<List<TagDto>> GetMyTags(JwtClaims claims);
}