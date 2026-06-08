using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectService projectService;

    public ProjectsController(IProjectService projectService)
    {
        this.projectService = projectService;
    }

    public async Task<IActionResult> All()
    {
        var userId = this.GetUserId();
        var projects = await this.projectService.GetMineAsync(userId);

        return View(projects);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var project = await this.projectService.GetDetailsAsync(id, userId, isAdmin);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    public IActionResult Create()
    {
        return View(new ProjectInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = this.GetUserId();

        await this.projectService.CreateAsync(model, userId);

        TempData["SuccessMessage"] = "Project created successfully.";

        return RedirectToAction(nameof(All));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var model = await this.projectService.GetForEditAsync(id, userId, isAdmin);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var edited = await this.projectService.EditAsync(id, model, userId, isAdmin);

        if (!edited)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Project updated successfully.";

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var project = await this.projectService.GetDetailsAsync(id, userId, isAdmin);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = this.GetUserId();
        var isAdmin = this.User.IsInRole("Administrator");

        var deleted = await this.projectService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Project deleted successfully.";

        return RedirectToAction(nameof(All));
    }

    private string GetUserId()
    {
        return this.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID was not found.");
    }
}