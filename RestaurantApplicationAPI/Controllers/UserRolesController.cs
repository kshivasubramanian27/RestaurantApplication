using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Extensions;
using RestaurantApplicationAPI.ServiceContracts;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;
        private readonly IPermissionService _permissionService;

        public UserRolesController(IUserRolesService userRolesService, IPermissionService permissionService)
        {
            _userRolesService = userRolesService;
            _permissionService = permissionService;
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
            //if (!User.HasPermission("Permission.View"))
            //    return Forbid();

            var permissions = await _userRolesService.GetAllPermissionsAsync();

            return Ok(permissions);
        }

        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission([FromBody] CreateUserPermissionDTO request)
        {
            //if (!User.HasPermission("Permission.Create"))
            //    return Forbid();

            var result = await _permissionService.CreatePermissionAsync(request);

            if (!result.Success)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(new { message = "User Permission Created sucessfully." });
        }

        [HttpGet("EditPermission/{id}")]
        public async Task<IActionResult> EditPermission(string id)
        {
            //if (!User.HasPermission("Permission.Update"))
            //    return Forbid();

            var result = await _permissionService.GetPermissionByIdAsync(id);

            if (!result.Success)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(result.Data);
        }

        [HttpPut("EditPermission/{id}")]
        public async Task<IActionResult> UpdatePermission(string id, [FromBody] EditUserPermissionDTO updatedPermissionDetails)
        {
            //if (!User.HasPermission("Permission.Update"))
            //    return Forbid();

            var result = await _permissionService.UpdatePermissionAsync(Guid.Parse(id), updatedPermissionDetails);

            if (!result.Success)
            {
                if (result.StatusCode == ServiceResultStatus.NotFound)
                    return NotFound(result);

                if (result.StatusCode == ServiceResultStatus.Conflict)
                    return Conflict(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpDelete("DeletePermission/{id}")]
        public async Task<IActionResult> DeletePermission(string id)
        {
            //if (!User.HasPermission("Permission.Delete"))
            //    return Forbid();

            var result = await _permissionService.DeletePermissionAsync(Guid.Parse(id));

            if (!result.Success)
                return StatusCode(500, result);

            return Ok(result);
        }
    }
}