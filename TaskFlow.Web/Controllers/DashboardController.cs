using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Web.Extensions;

namespace TaskFlow.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        this.dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await this.dashboardService.GetDashboardAsync(User.GetId());

        return View(model);
    }
}