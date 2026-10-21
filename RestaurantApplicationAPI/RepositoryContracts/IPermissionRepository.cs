using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IPermissionRepository
    {
        public Task<ServiceResult<IList<Permission>>> GetAllPermissionsAsync();

        public Task<IEnumerable<string>> GetPermissionsForRolesAsync(IEnumerable<string> roles);

        public Task<ServiceResult<Permission>> CreatePermissionAsync(Permission newPermission);

        public Task<ServiceResult<Permission>> GetPermissionDetailsAsync(string name);

        public Task<ServiceResult<Permission>> GetPermissionDetailsAsync(Guid Id);

        public Task<ServiceResult<Permission>> UpdatePermissionAsync(Permission permission);

        public Task<ServiceResult<bool>> DeletePermissionAsync(Guid Id);

        public Task<ServiceResult<string>> CreatePermissionForRole(RolePermission rolePermission);
    }
}