using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksApiController : ControllerBase
{
    private readonly ITaskService taskService;

    public TasksApiController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword = "")
    {
        var tasks = await this.taskService.SearchAsync(keyword, User.GetId());
        return Ok(tasks);
    }

    [HttpPost("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeTaskStatusInputModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await this.taskService.ChangeStatusAsync(id, model.Status, User.GetId(), User.IsInRole("Administrator"));
        return Ok(new { message = "Task status updated successfully." });
    }
}

public class ChangeTaskStatusInputModel
{
    [Required]
    public string Status { get; set; } = null!;
}
