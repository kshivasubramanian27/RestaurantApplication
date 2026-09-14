using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationUI.DTO.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using authService = RestaurantApplicationUI.ServiceContracts;

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

        [AllowAnonymous]
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

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(loginResponse.AccessToken);

            var claims = jwtToken.Claims.ToList();

            claims.Add(new Claim("access_token", loginResponse.AccessToken));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var accessToken = User.FindFirst("access_token")?.Value;

            if (!string.IsNullOrWhiteSpace(accessToken))
                await _authenticationService.LogoutAsync(accessToken);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}