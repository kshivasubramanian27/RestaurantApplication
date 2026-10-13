using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.ServiceContracts;

namespace RestaurantApplicationUI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class RolesAndPermissionsController : Controller
    {
        private readonly IUsersService _usersService;

        public RolesAndPermissionsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            ViewBag.allRoles = await _usersService.GetAllRolesAsync(accessToken);
            ViewBag.allPermissions = await _usersService.GetAllPermissionsAsync(accessToken);

            return View();
        }

        [HttpGet("AddPermissions")]
        public async Task<IActionResult> AddPermissions()
        {
            return View();
        }
    }
}