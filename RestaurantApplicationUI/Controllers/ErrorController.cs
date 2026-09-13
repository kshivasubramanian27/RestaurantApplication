using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Exception;

namespace RestaurantApplicationUI.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet("/Error")]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var controllerName = HttpContext.Items["OriginalControllerName"] as string ?? "Unknown";

            var exception = exceptionFeature?.Error;

            var exceptionInfo = new ExceptionInfo(controllerName, exception?.StackTrace ?? exception?.ToString() ?? "No stack trace available."
            );

            Response.StatusCode = 500;

            return View(exceptionInfo);
        }
    }
}