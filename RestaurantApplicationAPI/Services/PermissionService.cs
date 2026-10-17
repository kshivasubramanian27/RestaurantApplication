using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.Repository;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<ServiceResult<IList<UserPermissionsDTO>>> GetAllPermissionsAsync()
        {
            var allPermissions = await _permissionRepository.GetAllPermissionsAsync();

            if (!allPermissions.Success)
                return ServiceResult<IList<UserPermissionsDTO>>.FailureResult(allPermissions.Error, allPermissions.StatusCode);

            List <UserPermissionsDTO> permissions = new();

            foreach (Permission permission in allPermissions.Data)
            {
                permissions.Add(new UserPermissionsDTO
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                });
            }

            return ServiceResult<IList<UserPermissionsDTO>>.SuccessResult(permissions);
        }

        public async Task<ServiceResult<Permission>> CreatePermissionAsync(CreateUserPermissionDTO userPermission)
        {
            var permissionExists = await _permissionRepository.GetPermissionDetailsAsync(userPermission.Name);

            if (permissionExists.Success && permissionExists.Data != null)
                return ServiceResult<Permission>.FailureResult("Permission Already Exists", ServiceResultStatus.Conflict);

            if (permissionExists.StatusCode == ServiceResultStatus.Error)
                return ServiceResult<Permission>.FailureResult("Failed to check whether the permission exists.", ServiceResultStatus.Error);

            Permission newPermission = new()
            {
                Id = Guid.NewGuid(),
                Name = userPermission.Name,
                Description = userPermission.Description,
            };

            var result = await _permissionRepository.CreatePermissionAsync(newPermission);

            return result;
        }

        public async Task<ServiceResult<EditUserPermissionDTO>> GetPermissionByIdAsync(string Id)
        {
            if (!Guid.TryParse(Id, out Guid permissionId))
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Invalid permission ID", ServiceResultStatus.Error);

            var permission = await _permissionRepository.GetPermissionDetailsAsync(Guid.Parse(Id));

            if (permission.StatusCode == ServiceResultStatus.NotFound)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Permission doesn't exist with this ID", ServiceResultStatus.NotFound);

            if (!permission.Success || permission.Data == null)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Permission doesn't exist with this ID", ServiceResultStatus.Conflict);

            if (permission.StatusCode == ServiceResultStatus.Error)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Failed to get the permission data", ServiceResultStatus.Error);

            EditUserPermissionDTO permissionDTO = new()
            {
                Id = permission.Data.Id,
                Name = permission.Data.Name,
                Description = permission.Data.Description
            };

            return ServiceResult<EditUserPermissionDTO>.SuccessResult(permissionDTO);
        }

        public async Task<ServiceResult<EditUserPermissionDTO>> GetPermissionByNameAsync(string permissionName)
        {
            var permission = await _permissionRepository.GetPermissionDetailsAsync(permissionName);

            if (permission.StatusCode == ServiceResultStatus.NotFound)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Permission doesn't exist", ServiceResultStatus.NotFound);

            if (!permission.Success || permission.Data == null)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Permission doesn't exist", ServiceResultStatus.Conflict);

            if (permission.StatusCode == ServiceResultStatus.Error)
                return ServiceResult<EditUserPermissionDTO>.FailureResult("Failed to get the permission data", ServiceResultStatus.Error);

            EditUserPermissionDTO permissionDTO = new()
            {
                Id = permission.Data.Id,
                Name = permission.Data.Name,
                Description = permission.Data.Description
            };

            return ServiceResult<EditUserPermissionDTO>.SuccessResult(permissionDTO);
        }

        public async Task<ServiceResult<EditUserPermissionDTO>> UpdatePermissionAsync(Guid id, EditUserPermissionDTO updatedPermissionDetails)
        {
            var permission = new Permission
            {
                Id = id,
                Name = updatedPermissionDetails.Name,
                Description = updatedPermissionDetails.Description
            };

            var result = await _permissionRepository.UpdatePermissionAsync(permission);

            if (!result.Success || result.Data == null)
                return ServiceResult<EditUserPermissionDTO>.FailureResult(result.Error!, result.StatusCode);

            var updatedDto = new EditUserPermissionDTO
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description
            };

            return ServiceResult<EditUserPermissionDTO>.SuccessResult(updatedDto);
        }

        public async Task<ServiceResult<bool>> DeletePermissionAsync(Guid id)
        {
            var result = await _permissionRepository.DeletePermissionAsync(id);

            if (!result.Success)
                return ServiceResult<bool>.FailureResult("Failed to delete permission", result.StatusCode);

            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}