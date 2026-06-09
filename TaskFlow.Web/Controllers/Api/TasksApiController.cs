using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Services.Interfaces;

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

    [HttpPost("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeTaskStatusRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Status is required." });
        }

        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var isAdmin = this.User.IsInRole("Administrator");

        try
        {
            await this.taskService.ChangeStatusAsync(id, request.Status, userId, isAdmin);

            return Ok(new
            {
                message = "Task status updated successfully.",
                status = request.Status
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

public class ChangeTaskStatusRequest
{
    public string Status { get; set; } = null!;
}