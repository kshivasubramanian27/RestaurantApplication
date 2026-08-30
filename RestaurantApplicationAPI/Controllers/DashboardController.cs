using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        [Authorize]
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok(new
            {
                message = "JWT authentication is working."
            });
        }
    }
}