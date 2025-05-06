using System.ComponentModel.DataAnnotations;
using api.Extensions.Mappers;
using api.Models;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using efscaffold.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class TaskService(ISecurityService securityService, MyDbContext ctx, ILogger<TaskService> logger) : ITaskService
{
    public async Task<List<TickticktaskDto>> GetMyTasks(GetTasksFilterAndOrderParameters parameters,
        JwtClaims jwtClaims)
    {
        IQueryable<Tickticktask> query = ctx.Tickticktasks.Include(t => t.TaskTags);
        if (parameters.ListIds != null && parameters.ListIds?.Count > 0)
            //Logical OR inclusion (list belongs to user, so no need to check for user)
            query = query.Where(task => parameters.ListIds.Contains(task.ListId));

        if (parameters.MinPriority != null) query = query.Where(t => t.Priority >= parameters.MinPriority);

        if (parameters.MaxPriority != null) query = query.Where(t => t.Priority <= parameters.MaxPriority);

        if (parameters.IsCompleted != null) query = query.Where(t => t.Completed == parameters.IsCompleted);

        if (parameters.LatestDueDate != null) query = query.Where(t => t.DueDate < parameters.LatestDueDate);

        if (parameters.EarliestDueDate != null) query = query.Where(t => t.DueDate > parameters.EarliestDueDate);

        if (parameters.TagIds != null && parameters.TagIds.Count > 0)
            //logical OR inclusion
            query = query
                .Where(t => parameters.TagIds
                    .Any(f => t.TaskTags.Select(tag => tag.TagId)
                        .Contains(f))
                );


        if (parameters.SearchTerm != null)
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(searchTerm) || t.Description.ToLower().Contains(searchTerm));
        }

        if (parameters.OrderBy != null)
        {
            if (parameters.OrderBy == nameof(Tickticktask.Priority))
                query = query.OrderBy(t => t.Priority);
            if (parameters.OrderBy == nameof(Tickticktask.DueDate))
                query = query.OrderBy(t => t.DueDate);
        }

        if (parameters.IsDescending != null) query = query.Reverse();

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
        var existing = ctx.Tickticktasks
            .Include(t => t.TaskTags)
            .First(t => t.TaskId == dto.Id);

        var newList = ctx.Tasklists.First(list => list.ListId == dto.ListId);

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.DueDate = dto.DueDate;
        existing.Priority = dto.Priority;
        existing.List = newList;
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

    public async Task<TasklistDto> CreateList(JwtClaims claims, CreateListRequestDto dto)
    {
        var tasList = new Tasklist
        {
            CreatedAt = DateTime.UtcNow,
            ListId = Guid.NewGuid().ToString(),
            Name = dto.ListName,
            UserId = claims.Id
        };
        ctx.Tasklists.Add(tasList);
        ctx.SaveChanges();
        return tasList.ToDto();
    }

    public async Task<TagDto> CreateTag(JwtClaims claims, CreateTagRequestDto dto)
    {
        var tag = new Tag
        {
            CreatedAt = DateTime.UtcNow,
            Name = dto.TagName,
            UserId = claims.Id,
            TagId = Guid.NewGuid().ToString()
        };
        ctx.Tags.Add(tag);
        ctx.SaveChanges();
        return tag.ToDto();
    }

    public async Task<TasklistDto> UpdateList(JwtClaims claims, UpdateListRequestDto dto)
    {
        var taskList = ctx.Tasklists.FirstOrDefault(t => t.ListId == dto.ListId) ??
                       throw new Exception("Could not find list with id " + dto.ListId);
        taskList.Name = dto.NewName;
        ctx.Tasklists.Update(taskList);
        ctx.SaveChanges();
        return taskList.ToDto();
    }

    public async Task<TagDto> UpdateTag(JwtClaims claims, UpdateTagRequestDto dto)
    {
        var tag = ctx.Tags.FirstOrDefault(t => t.TagId == dto.TagId) ??
                  throw new Exception("Could not find tag with ID " + dto.TagId);
        tag.Name = dto.NewName;
        ctx.Tags.Update(tag);
        ctx.SaveChanges();
        return tag.ToDto();
    }

    public async Task DeleteListWithAllTasks(string listId, JwtClaims claims)
    {
        var tasks = ctx.Tickticktasks.Where(t => t.ListId == listId).ToList();
        foreach (var task in tasks) ctx.Tickticktasks.Remove(task);
        ctx.SaveChanges();
        var list = ctx.Tasklists.First(t => t.ListId == listId);
        ctx.Tasklists.Remove(list);
        ctx.SaveChanges();
    }

    public async Task DeleteTag(string tagId, JwtClaims claims)
    {
        var tag = ctx.Tags.First(t => t.TagId == tagId);
        ctx.Tags.Remove(tag);
        ctx.SaveChanges();
    }

    public async Task<TaskTagDto> AddTagToTask(JwtClaims claims, ChangeTaskTagRequestDto dto)
    {
        var existingTag = ctx.TaskTags.FirstOrDefault(t => t.TagId == dto.TagId && t.TaskId == dto.TaskId);

        if (existingTag != null)
            throw new Exception("Task already has this tag");


        var taskTag = new TaskTag
        {
            CreatedAt = DateTime.UtcNow,
            TagId = dto.TagId,
            TaskId = dto.TaskId
        };
        ctx.TaskTags.Add(taskTag);
        ctx.SaveChanges();
        return taskTag.ToDto();
    }

    public async Task RemoveTaskTag(JwtClaims claims, ChangeTaskTagRequestDto dto)
    {
        var taskTag = ctx.TaskTags.FirstOrDefault(t => t.TagId == dto.TagId && t.TaskId == dto.TaskId);
        if (taskTag == null)
            throw new Exception("Task does not have this tag");
        ctx.TaskTags.Remove(taskTag);
        ctx.SaveChanges();
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
    Task<TasklistDto> CreateList(JwtClaims claims, CreateListRequestDto dto);
    Task<TagDto> CreateTag(JwtClaims claims, CreateTagRequestDto dto);
    Task<TasklistDto> UpdateList(JwtClaims claims, UpdateListRequestDto dto);
    Task<TagDto> UpdateTag(JwtClaims claims, UpdateTagRequestDto dto);

    Task DeleteListWithAllTasks(string listId, JwtClaims claims);
    Task DeleteTag(string tagId, JwtClaims claims);
    Task<TaskTagDto> AddTagToTask(JwtClaims claims, ChangeTaskTagRequestDto dto);
    Task RemoveTaskTag(JwtClaims claims, ChangeTaskTagRequestDto dto);
}