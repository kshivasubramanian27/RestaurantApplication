using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Extensions
{
    public static class ServiceResultExtension
    {
        public static IActionResult ToActionResult<T>(this ServiceResult<T> result, ControllerBase controller)
        {
            return result.StatusCode switch
            {
                ServiceResultStatus.Success =>
                    controller.Ok(result.Data),

                ServiceResultStatus.ValidationError =>
                    controller.BadRequest(new
                    {
                        message = result.Error
                    }),

                ServiceResultStatus.Unauthorized =>
                    controller.Unauthorized(new
                    {
                        message = result.Error
                    }),

                ServiceResultStatus.Forbidden =>
                    controller.Forbid(),

                ServiceResultStatus.NotFound =>
                    controller.NotFound(new
                    {
                        message = result.Error
                    }),

                ServiceResultStatus.Conflict =>
                    controller.Conflict(new
                    {
                        message = result.Error
                    }),

                _ =>
                    controller.StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new
                        {
                            message = result.Error
                        })
            };
        }
    }
}