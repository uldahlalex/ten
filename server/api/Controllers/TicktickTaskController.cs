using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class TicktickTaskController(
    ITaskService taskService,
    ILogger<TicktickTaskController> logger) : Controller
{
    [HttpPost]
    [Route(nameof(GetMyTasks))]
    public async Task<ActionResult<List<TickticktaskDto>>> GetMyTasks(
        [FromBody] TaskFilteringAndSorting parameters)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;

        var result = await taskService.GetMyTasks(parameters, requesterId);
        return result;
    }


    [HttpPost]
    [Route(nameof(CreateTask))]
    public async Task<ActionResult<TickticktaskDto>> CreateTask(
        [FromBody] CreateTaskRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.CreateTask(dto, requesterId);
        return result;
    }

    [HttpPatch]
    [Route(nameof(UpdateTask))]
    public async Task<ActionResult<TickticktaskDto>> UpdateTask(
        [FromBody] UpdateTaskRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.UpdateTask(dto, requesterId);
        return result;
    }

    [HttpDelete]
    [Route(nameof(DeleteTask))]
    public async Task<ActionResult<TickticktaskDto>> DeleteTask(
        [FromQuery] string taskId)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        await taskService.DeleteTask(taskId, requesterId);
        return Ok();
    }

    [HttpGet]
    [Route(nameof(GetMyTags))]
    public async Task<ActionResult<List<TagDto>>> GetMyTags()
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.GetMyTags(requesterId);
        return result;
    }

    [HttpGet]
    [Route(nameof(GetMyLists))]
    public async Task<ActionResult<List<TasklistDto>>> GetMyLists()
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.GetMyLists(requesterId);
        return result;
    }

    [HttpPost]
    [Route(nameof(CreateList))]
    public async Task<ActionResult<TasklistDto>> CreateList(
        [FromBody] CreateListRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.CreateList(requesterId, dto);
        return result;
    }

    [HttpPost]
    [Route(nameof(CreateTag))]
    public async Task<ActionResult<TagDto>> CreateTag(
        [FromBody] CreateTagRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.CreateTag(requesterId, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(UpdateList))]
    public async Task<ActionResult<TasklistDto>> UpdateList(
        [FromBody] UpdateListRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.UpdateList(requesterId, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(UpdateTag))]
    public async Task<ActionResult<TagDto>> UpdateTag(
        [FromBody] UpdateTagRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.UpdateTag(requesterId, dto);
        return result;
    }

    [HttpDelete]
    [Route(nameof(DeleteListWithTasks))]
    public async Task<ActionResult> DeleteListWithTasks(
        [FromQuery] string listId)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        await taskService.DeleteListWithAllTasks(listId, requesterId);
        return Ok();
    }

    [HttpDelete]
    [Route(nameof(DeleteTag))]
    public async Task<ActionResult> DeleteTag(
        [FromQuery] string tagId)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        await taskService.DeleteTag(tagId, requesterId);
        return Ok();
    }

    [HttpPut]
    [Route(nameof(AddTaskTag))]
    public async Task<ActionResult<TaskTagDto>> AddTaskTag(
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        var result = await taskService.AddTagToTask(requesterId, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(RemoveTaskTag))]
    public async Task<ActionResult> RemoveTaskTag(
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var requesterId = User.FindFirst(nameof(JwtClaims.Id))!.Value;
        await taskService.RemoveTaskTag(requesterId, dto);
        return Ok();
    }
}