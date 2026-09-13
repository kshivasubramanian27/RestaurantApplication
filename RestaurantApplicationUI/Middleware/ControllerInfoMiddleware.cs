using Microsoft.AspNetCore.Mvc.Controllers;

namespace RestaurantApplicationUI.Middleware
{
    public class ControllerInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public ControllerInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Items.ContainsKey("OriginalControllerName"))
            {
                var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    var controllerType = actionDescriptor.ControllerTypeInfo.AsType();

                    var controllerName = controllerType.Name.Replace("Controller", "");

                    context.Items["OriginalControllerName"] = controllerName;
                }
            }

            await _next(context);
        }
    }
}