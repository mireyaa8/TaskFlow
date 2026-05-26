using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class BoardsController : Controller
{
    private readonly IBoardService boardService;
    private readonly ITaskService taskService;

    public BoardsController(IBoardService boardService, ITaskService taskService)
    {
        this.boardService = boardService;
        this.taskService = taskService;
    }

    public async Task<IActionResult> Details(int id)
    {
        var board = await this.boardService.GetByIdAsync(id, User.GetId());
        if (board == null) return NotFound();

        ViewBag.Tasks = await this.taskService.GetByBoardAsync(id, User.GetId());
        return View(board);
    }

    public IActionResult Create(int projectId)
    {
        return View(new BoardInputModel { ProjectId = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BoardInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await this.boardService.CreateAsync(model, User.GetId());
        TempData["SuccessMessage"] = "Board created successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
