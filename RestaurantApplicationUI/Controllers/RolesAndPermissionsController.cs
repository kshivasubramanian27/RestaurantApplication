using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Roles;
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

        [HttpPost("AddPermission")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPermission(CreateUserPermissionDTO model)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.CreatePermissionError = "Unable to create new user permission.";

                return View("AddPermissions", model);
            }

            var result = await _usersService.CreatePermissionAsync(accessToken, model);

            if (!result.success)
            {
                ViewBag.CreatePermissionError = result.error;

                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("EditPermission/{id}")]
        public async Task<IActionResult> EditPermission(string id)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var permissionDetails = await _usersService.GetPermissionDetailsByIdAsync(accessToken, id);

            return View(permissionDetails);
        }

        [HttpPost("EditPermission/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermission(string id, EditUserPermissionDTO model)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var result = await _usersService.UpdatePermissionDetailsAsync(accessToken, model);

            if(!result.success)
            {
                ViewBag.ErrorDetails = $"{result.error} occured";

                return RedirectToAction($"EditPermission/{model.Id}");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("DeletePermission/{id}")]
        public async Task<IActionResult> DeletePermission(string id)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var result = await _usersService.DeletePermissionAsync(accessToken, Guid.Parse(id));

            if (!result.success)
                ViewBag.ErrorDetails = $"{result.error} occured";

            return RedirectToAction(nameof(Index));
        }
    }
}