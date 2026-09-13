using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.Extensions;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if(!User.HasPermission("Users.View"))
                Forbid();

            var allUsers = await _usersService.GetAllUsers();

            return Ok(allUsers);
        }
    }
}