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
        var labels = await this.labelService.GetAllAsync();

        return View(labels);
    }

    public IActionResult Create()
    {
        return View(new LabelInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LabelInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await this.labelService.CreateAsync(model);

        TempData["SuccessMessage"] = "Label created successfully.";

        return RedirectToAction(nameof(All));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await this.labelService.GetForEditAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LabelInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var edited = await this.labelService.EditAsync(id, model);

        if (!edited)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Label updated successfully.";

        return RedirectToAction(nameof(All));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await this.labelService.GetForEditAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.LabelId = id;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await this.labelService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Label deleted successfully.";

        return RedirectToAction(nameof(All));
    }
}