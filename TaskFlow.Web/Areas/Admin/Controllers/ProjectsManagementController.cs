using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Web.Extensions;

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
        // Starter version shows current admin-accessible projects. Add a true GetAllAdminAsync later.
        return View(await this.projectService.GetMineAsync(User.GetId()));
    }
}
