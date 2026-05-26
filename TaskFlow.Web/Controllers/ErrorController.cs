using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Web.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        return statusCode switch
        {
            400 => View("BadRequest"),
            401 => View("Unauthorized"),
            404 => View("NotFound"),
            _ => View("InternalServerError")
        };
    }
}
