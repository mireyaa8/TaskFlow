using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About() => View();
}
