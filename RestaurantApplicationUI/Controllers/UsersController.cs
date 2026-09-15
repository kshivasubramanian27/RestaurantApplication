using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Users;
using RestaurantApplicationUI.ServiceContracts;
using System.Security.Claims;

namespace RestaurantApplicationUI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var users = await _usersService.GetAllUsersAsync(accessToken);

            ViewBag.ActiveMenu = "Users";

            return View(users);
        }

        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var roles = await _usersService.GetAllRolesAsync(accessToken);

            ViewBag.Roles = roles;

            return View(new CreateUserRequestDTO());
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateUserRequestDTO request)
        {
            if (request.Password != request.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");

            if (!ModelState.IsValid)
            {
                await LoadRoles(request);

                return View(request);
            }

            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var result = await _usersService.CreateUserAsync(request, accessToken);

            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.error ?? "Unable to create the user.");

                await LoadRoles(request);

                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            var user = await _usersService.GetUserByIdAsync(accessToken, id);

            var roles = await _usersService.GetAllRolesAsync(accessToken);

            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateUserDTO model)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return RedirectToAction("Login", "Account");

            model.Id = id;

            if (!ModelState.IsValid)
            {
                var roles = await _usersService.GetAllRolesAsync(accessToken);

                ViewBag.Roles = roles;

                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                ViewBag.CurrentUserRole = currentUserRole;

                var existingUser = await _usersService.GetUserByIdAsync(accessToken, id);

                return View(existingUser);
            }

            var result = await _usersService.UpdateUserAsync(accessToken, model);

            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.error ?? "Unable to update the user.");

                var roles = await _usersService.GetAllRolesAsync(accessToken);

                ViewBag.Roles = roles;

                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                ViewBag.CurrentUserRole = currentUserRole;

                var existingUser = await _usersService.GetUserByIdAsync(accessToken, id);

                return View(existingUser);
            }

            return RedirectToAction(nameof(Index));
        }

        #region Private method declarations

        private async Task LoadRoles(CreateUserRequestDTO request)
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(accessToken))
                return;

            var roles = await _usersService.GetAllRolesAsync(accessToken);

            ViewBag.Roles = roles;
        }

        #endregion
    }
}