using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Extensions;
using RestaurantApplicationAPI.ServiceContracts;
using System.Security.Claims;

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
            if (!User.HasPermission("Role.View"))
                return Forbid();

            var allRoles = await _userRolesService.GetAllRolesAsync();

            return Ok(allRoles);
        }

        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            if (!User.HasPermission("Permission.View"))
                return Forbid();

            var permissions = await _permissionService.GetAllPermissionsAsync();

            return permissions.ToActionResult(this);
        }

        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission([FromBody] CreateUserPermissionDTO request)
        {
            if (!User.HasPermission("Permission.Create"))
                return Forbid();

            var result = await _permissionService.CreatePermissionAsync(request);

            return result.ToActionResult(this);
        }

        [HttpGet("EditPermission/{id}")]
        public async Task<IActionResult> EditPermission(string id)
        {
            if (!User.HasPermission("Permission.Update"))
                return Forbid();

            var result = await _permissionService.GetPermissionByIdAsync(id);

            return result.ToActionResult(this);
        }

        [HttpPut("EditPermission/{id}")]
        public async Task<IActionResult> UpdatePermission(string id, [FromBody] EditUserPermissionDTO updatedPermissionDetails)
        {
            if (!User.HasPermission("Permission.Update"))
                return Forbid();

            var result = await _permissionService.UpdatePermissionAsync(Guid.Parse(id), updatedPermissionDetails);

            return result.ToActionResult(this);
        }

        [HttpDelete("DeletePermission/{id}")]
        public async Task<IActionResult> DeletePermission(string id)
        {
            if (!User.HasPermission("Permission.Delete"))
                return Forbid();

            var result = await _permissionService.DeletePermissionAsync(Guid.Parse(id));

            return result.ToActionResult(this);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(CreateUserRoleDTO userRole)
        {
            if (!User.HasPermission("Role.Add"))
                return Forbid();

            var result = await _userRolesService.CreateUserRoleAsync(userRole);

            return result.ToActionResult(this);
        }

        [HttpGet("EditRole/{id}")]
        public async Task<IActionResult> EditRole(string id)
        {
            if (!User.HasPermission("Role.Update"))
                return Forbid();

            var result = await _userRolesService.GetUserRoleDetailsAsync(id);

            return result.ToActionResult(this);
        }

        [HttpPut("EditRole/{id}")]
        public async Task<IActionResult> EditRole(string id, [FromBody] EditUserRoleDTO editRoleDTO)
        {
            if (!User.HasPermission("Role.Update"))
                return Forbid();

            var result = await _userRolesService.UpdateRoleDetailsAsync(id, editRoleDTO);

            return result.ToActionResult(this);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (!User.HasPermission("Role.Delete"))
                return Forbid();

            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(currentUserRole))
                return Forbid();

            var result = await _userRolesService.DeleteRoleAsync(id, currentUserRole);

            return result.ToActionResult(this);
        }
    }
}