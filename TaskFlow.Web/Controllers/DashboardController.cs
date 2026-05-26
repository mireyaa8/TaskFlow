using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProjectService projectService;
    private readonly ITaskService taskService;

    public DashboardController(IProjectService projectService, ITaskService taskService)
    {
        this.projectService = projectService;
        this.taskService = taskService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Projects = await this.projectService.GetMineAsync(User.GetId());
        ViewBag.Tasks = await this.taskService.GetMineAsync(User.GetId());
        return View();
    }
}
