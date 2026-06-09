using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class CommentsController : Controller
{
    private readonly ICommentService commentService;

    public CommentsController(ICommentService commentService)
    {
        this.commentService = commentService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommentInputModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Comment must be between 2 and 1000 characters.";
            return RedirectToAction("Details", "Tasks", new { id = model.TaskItemId });
        }

        var userId = this.GetUserId();

        var created = await this.commentService.CreateAsync(model, userId);

        if (!created)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Comment added successfully.";

        return RedirectToAction("Details", "Tasks", new { id = model.TaskItemId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int taskId)
    {
        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var deleted = await this.commentService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Comment deleted successfully.";

        return RedirectToAction("Details", "Tasks", new { id = taskId });
    }

    private string GetUserId()
    {
        return this.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID was not found.");
    }
}