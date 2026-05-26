using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Services.Interfaces;
using TaskFlow.Services.Models;

namespace TaskFlow.Web.Controllers;

[Authorize(Roles = "Administrator")]
public class LabelsController : Controller
{
    private readonly ILabelService labelService;

    public LabelsController(ILabelService labelService)
    {
        this.labelService = labelService;
    }

    public async Task<IActionResult> All()
    {
        return View(await this.labelService.GetAllAsync());
    }

    public IActionResult Create() => View(new LabelInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LabelInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await this.labelService.CreateAsync(model);
        TempData["SuccessMessage"] = "Label created successfully.";
        return RedirectToAction(nameof(All));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await this.labelService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Label deleted successfully.";
        return RedirectToAction(nameof(All));
    }
}
