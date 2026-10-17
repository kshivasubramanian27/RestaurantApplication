using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Exception;
using RestaurantApplicationUI.Exceptions;

namespace RestaurantApplicationUI.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet("/Error")]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var exception = exceptionFeature?.Error;

            var model = new ExceptionInfo(
                controllerName: exceptionFeature?.Path ?? "Unknown",
                errorMessage: "An unexpected error occurred. Please try again later.",
                statusCode: 500,
                stackTrace: ""
            );

            if (exception is ApiRequestException apiException)
            {
                model = new ExceptionInfo(
                    controllerName: exceptionFeature?.Path ?? "Unknown",
                    errorMessage: apiException.Message,
                    statusCode: (int)apiException.StatusCode,
                    stackTrace: apiException.StackTrace!
                );
            }

            return View(model);
        }
    }
}