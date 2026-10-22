using Microsoft.AspNetCore.Identity;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IUserRolesRepository
    {
        public Task<IList<ApplicationRole>> GetAllRolesAsync();

        public Task<ServiceResult<ApplicationRole>> GetRoleDetailsAsync(string roleName);

        public Task<ServiceResult<ApplicationRole>> GetRoleDetailsAsync(Guid id);

        public Task<ServiceResult<IdentityResult>> CreateRoleAsync(ApplicationRole newRole);

        public Task<ServiceResult<IList<RolePermission>>> GetPermissionsForRoleAsync(string roleId);

        public Task<ServiceResult<ApplicationRole>> UpdateRoleAsync(ApplicationRole role);

        public Task<ServiceResult<bool>> DeleteRolePermissionAsync(RolePermission rolePermission);

        public Task<ServiceResult<bool>> DeleteRoleAsync(ApplicationRole role);
    }
}