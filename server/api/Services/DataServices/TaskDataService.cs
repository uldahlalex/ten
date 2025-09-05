using System.ComponentModel.DataAnnotations;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class TaskDataService(MyDbContext ctx, TimeProvider timeProvider) : ITaskDataService
{
    public Task<List<TickticktaskDto>> GetMyTasks(MyAmazingFilteringStuff parameters, string userId)
    {
        IQueryable<Tickticktask> query = ctx.Tickticktasks.Include(t => t.TaskTags);

        //Filter by user
        query = query
            .Where(task => task.List.UserId == userId);
        
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
            else if (parameters.OrderBy == nameof(Tickticktask.DueDate))
                query = query.OrderBy(t => t.DueDate);
            else if (parameters.OrderBy == nameof(Tickticktask.CreatedAt))
                query = query.OrderBy(t => t.CreatedAt);
            else
                throw new ValidationException("Not a valid ordering!");
        }

        if (parameters.IsDescending != null) query = query.Reverse();

        return Task.FromResult(query.Select(t => t.ToDto()).ToList());
    }

    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, string userId)
    {
        if (dto.DueDate != null && dto.DueDate < timeProvider.GetUtcNow().UtcDateTime)
            throw new ValidationException("Due date cannot be in the past");

        var tags = dto.TagsIds.Select(tagId =>
            ctx.TaskTags.First(tag => tag.TagId == tagId)).ToList();
        var createdAt = timeProvider.GetUtcNow().UtcDateTime;


        var entity = new Tickticktask(createdAt, dto.ListId, dto.Title, dto.Description, dto.DueDate, dto.Priority, false, null);

        ctx.Tickticktasks.Add(entity);
        ctx.SaveChanges();
        return Task.FromResult(entity.ToDto());
    }

    public Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, string userId)
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
        existing.CompletedAt = dto.Completed ? timeProvider.GetUtcNow().UtcDateTime : null;


        ctx.Tickticktasks.Update(existing);
        ctx.SaveChanges();

        return Task.FromResult(existing.ToDto());
    }

    public Task DeleteTask(string taskId, string userId)
    {
        var task = ctx.Tickticktasks.First(t => t.TaskId == taskId);
        ctx.Tickticktasks.Remove(task);
        ctx.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<List<TasklistDto>> GetMyLists(string userId)
    {
        var lists = ctx.Tasklists
            .Where(list => list.UserId == userId)
            .Select(list => list.ToDto())
            .ToList();
        return Task.FromResult(lists);
    }

    public Task<List<TagDto>> GetMyTags(string userId)
    {
        var tags = ctx.Tags
            .Where(tag => tag.UserId == userId)
            .Select(tag => tag.ToDto())
            .ToList();
        return Task.FromResult(tags);
    }

    public Task<TasklistDto> CreateList(string userId, CreateListRequestDto dto)
    {
        var myLists = ctx.Tasklists.Where(l => l.UserId == userId);
        if (myLists.Any(l => l.Name == dto.ListName))
            throw new ValidationException("List with this name already exists");
        var createdAt = timeProvider.GetUtcNow().UtcDateTime;

        var taskList = new Tasklist(createdAt, dto.ListName, userId);
        ctx.Tasklists.Add(taskList);
        ctx.SaveChanges();
        return Task.FromResult(taskList.ToDto());
    }

    public Task<TagDto> CreateTag(string userId, CreateTagRequestDto dto)
    {

        var tag = new Tag(timeProvider.GetUtcNow().UtcDateTime,dto.TagName, userId);
        ctx.Tags.Add(tag);
        ctx.SaveChanges();
        return Task.FromResult(tag.ToDto());
    }

    public Task<TasklistDto> UpdateList(string userId, UpdateListRequestDto dto)
    {
        var taskList = ctx.Tasklists.FirstOrDefault(t => t.ListId == dto.ListId) ??
                       throw new ValidationException("Could not find list with id " + dto.ListId);
        taskList.Name = dto.NewName;
        ctx.Tasklists.Update(taskList);
        ctx.SaveChanges();
        return Task.FromResult(taskList.ToDto());
    }

    public Task<TagDto> UpdateTag(string userId, UpdateTagRequestDto dto)
    {
        var tag = ctx.Tags.FirstOrDefault(t => t.TagId == dto.TagId) ??
                  throw new ValidationException("Could not find tag with ID " + dto.TagId);
        tag.Name = dto.NewName;
        ctx.Tags.Update(tag);
        ctx.SaveChanges();
        return Task.FromResult(tag.ToDto());
    }

    public Task DeleteListWithAllTasks(string listId, string userId)
    {
        var tasks = ctx.Tickticktasks.Where(t => t.ListId == listId).ToList();
        foreach (var task in tasks) ctx.Tickticktasks.Remove(task);
        ctx.SaveChanges();
        var list = ctx.Tasklists.First(t => t.ListId == listId);
        ctx.Tasklists.Remove(list);
        ctx.SaveChanges();
        return Task.CompletedTask;
    }

    public Task DeleteTag(string tagId, string userId)
    {
        var tag = ctx.Tags.First(t => t.TagId == tagId);
        ctx.Tags.Remove(tag);
        ctx.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<TaskTagDto> AddTagToTask(string userId, ChangeTaskTagRequestDto dto)
    {
        var existsAlready = ctx.TaskTags.Any(t => t.TagId == dto.TagId && t.TaskId == dto.TaskId);

        if (existsAlready)
            throw new ValidationException("Task already has this tag");

        var createdAt =  timeProvider.GetUtcNow().UtcDateTime;

        var taskTag = new TaskTag(createdAt, dto.TaskId, dto.TagId);
        ctx.TaskTags.Add(taskTag);
        ctx.SaveChanges();
        return Task.FromResult(taskTag.ToDto());
    }

    public Task RemoveTaskTag(string userId, ChangeTaskTagRequestDto dto)
    {
        var taskTag = ctx.TaskTags.FirstOrDefault(t => t.TagId == dto.TagId && t.TaskId == dto.TaskId);
        if (taskTag == null)
            throw new ValidationException("Task does not have this tag");
        ctx.TaskTags.Remove(taskTag);
        ctx.SaveChanges();
        return Task.CompletedTask;
    }
}