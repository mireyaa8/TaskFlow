using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectService projectService;
    private readonly IBoardService boardService;

    public ProjectsController(IProjectService projectService, IBoardService boardService)
    {
        this.projectService = projectService;
        this.boardService = boardService;
    }

    public async Task<IActionResult> All()
    {
        var projects = await this.projectService.GetMineAsync(User.GetId());
        return View(projects);
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await this.projectService.GetByIdAsync(id, User.GetId());
        if (project == null) return NotFound();

        ViewBag.Boards = await this.boardService.GetByProjectAsync(id, User.GetId());
        return View(project);
    }

    public IActionResult Create() => View(new ProjectInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await this.projectService.CreateAsync(model, User.GetId());
        TempData["SuccessMessage"] = "Project created successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var project = await this.projectService.GetByIdAsync(id, User.GetId());
        if (project == null) return NotFound();

        return View(new ProjectInputModel { Name = project.Name, Description = project.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await this.projectService.EditAsync(id, model, User.GetId());
        TempData["SuccessMessage"] = "Project updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await this.projectService.DeleteAsync(id, User.GetId());
        TempData["SuccessMessage"] = "Project deleted successfully.";
        return RedirectToAction(nameof(All));
    }
}
