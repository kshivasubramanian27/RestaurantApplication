using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IPermissionService
    {
        public Task<ServiceResult<IList<UserPermissionsDTO>>> GetAllPermissionsAsync();

        public Task<ServiceResult<Permission>> CreatePermissionAsync(CreateUserPermissionDTO userPermission);

        public Task<ServiceResult<EditUserPermissionDTO>> GetPermissionByIdAsync(string Id);

        public Task<ServiceResult<EditUserPermissionDTO>> GetPermissionByNameAsync(string permissionName);

        public Task<ServiceResult<EditUserPermissionDTO>> UpdatePermissionAsync(Guid id, EditUserPermissionDTO updatedPermissionDetails);

        public Task<ServiceResult<bool>> DeletePermissionAsync(Guid id);
    }
}