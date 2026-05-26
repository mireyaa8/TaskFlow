using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ITaskService taskService;

    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    public async Task<IActionResult> MyTasks()
    {
        var tasks = await this.taskService.GetMineAsync(User.GetId());
        return View(tasks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var task = await this.taskService.GetByIdAsync(id, User.GetId());
        if (task == null) return NotFound();

        return View(task);
    }

    public IActionResult Create(int boardId)
    {
        return View(new TaskInputModel { BoardId = boardId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await this.taskService.CreateAsync(model, User.GetId());
        TempData["SuccessMessage"] = "Task created successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var task = await this.taskService.GetByIdAsync(id, User.GetId());
        if (task == null) return NotFound();

        return View(new TaskInputModel
        {
            BoardId = task.BoardId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await this.taskService.EditAsync(id, model, User.GetId());
        TempData["SuccessMessage"] = "Task updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await this.taskService.DeleteAsync(id, User.GetId());
        TempData["SuccessMessage"] = "Task deleted successfully.";
        return RedirectToAction(nameof(MyTasks));
    }
}
