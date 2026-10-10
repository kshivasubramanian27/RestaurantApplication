using Microsoft.AspNetCore.Mvc;

namespace RestaurantApplicationAPI.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                if (context.Response.HasStarted)
                    throw;

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = exception.Message
                };

                await context.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken: context.RequestAborted);
            }
        }
    }
}