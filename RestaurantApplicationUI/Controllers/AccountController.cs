using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Authentication;
using authService = RestaurantApplicationUI.ServiceContracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RestaurantApplicationUI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly authService.IAuthenticationService _authenticationService;

        public AccountController(authService.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var loginResponse = await _authenticationService.LoginAsync(request);

            if (loginResponse == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

                return View(request);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, request.Username) };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}