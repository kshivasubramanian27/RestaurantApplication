using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.Extensions;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public UserRolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            if (!User.HasPermission("User.Create"))
                return Forbid();

            var allRoles = await _userRolesService.GetAllRolesAsync();

            return Ok(allRoles);
        }

        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            //if (!User.HasPermission("User.PermissionCreate"))
            //    return Forbid();

            var permissions = await _userRolesService.GetAllPermissionsAsync();

            return Ok(permissions);
        }
    }
}