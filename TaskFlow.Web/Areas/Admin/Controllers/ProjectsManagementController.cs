using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class ProjectsManagementController : Controller
{
    private readonly IProjectService projectService;

    public ProjectsManagementController(IProjectService projectService)
    {
        this.projectService = projectService;
    }

    public async Task<IActionResult> All()
    {
        var projects = await this.projectService.GetAllAsync();

        return View(projects);
    }
}