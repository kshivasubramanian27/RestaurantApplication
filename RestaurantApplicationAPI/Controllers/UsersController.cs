using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.DTO.Users;
using RestaurantApplicationAPI.Extensions;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            if(!User.HasPermission("User.View"))
                return Forbid();

            var allUsers = await _usersService.GetAllUsers();

            return Ok(allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDTO request)
        {
            if (!User.HasPermission("User.Create"))
                return Forbid();

            var result = await _usersService.CreateUserAsync(request, User.Identity?.Name);

            if (!result.Success)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(new
            {
                message = "User created successfully."
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> EditUser(string userId)
        {
            if (!User.HasPermission("User.Update"))
                return Forbid();

            var result = await _usersService.GetUserByIdAsync(userId);

            return Ok(result);
        }
    }
}