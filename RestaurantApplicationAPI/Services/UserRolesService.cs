using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.Repository;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Services
{
    public class UserRolesService : IUserRolesService
    {
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IPermissionRepository _permissionRepository;

        public UserRolesService(IUserRolesRepository userRolesRepository, IPermissionRepository permissionRepository)
        {
            _userRolesRepository = userRolesRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<IList<UserRolesDTO>> GetAllRolesAsync()
        {
            var allRoles = await _userRolesRepository.GetAllRolesAsync();

            List<UserRolesDTO> userRoles = new();

            foreach (var role in allRoles)
            {
                userRoles.Add(
                    new UserRolesDTO
                        {
                            Id = role.Id,
                            RoleName = role.Name,
                            RoleDescrption = role.Description,
                            NormalisedName = role.NormalizedName
                        }
                    );
            }

            return userRoles;
        }

        public async Task<ServiceResult<string>> CreateUserRoleAsync(CreateUserRoleDTO userRole)
        {
            var result = await _userRolesRepository.GetRoleDetailsAsync(userRole.Name);

            if (result.Data != null)
                return ServiceResult<string>.FailureResult(result.Error, result.StatusCode);

            ApplicationRole newRole = new()
            {
                Name = userRole.Name,
                Description = userRole.Description,
                NormalizedName = userRole.NormalisedName
            };

            var createdRole = await _userRolesRepository.CreateRoleAsync(newRole);

            if (!createdRole.Success)
                return ServiceResult<string>.FailureResult(createdRole.Error, createdRole.StatusCode);

            var allPermissions = await _permissionRepository.GetAllPermissionsAsync();
            var allPermissionsList = allPermissions.Data.Select(x => x.Id).ToHashSet();

            var incomingPermissionList = userRole.Permissions
                .Select(x => Guid.TryParse(x, out Guid id) ? id : (Guid?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Where(id => allPermissionsList.Contains(id))
                .ToList();

            foreach( var incomingPermission in incomingPermissionList)
            {
                RolePermission rolePermission = new()
                {
                    RoleId = newRole.Id,
                    PermissionId = incomingPermission
                };

                var createdRolePermission = await _permissionRepository.CreatePermissionForRole(rolePermission);

                if (!createdRolePermission.Success)
                    return ServiceResult<string>.FailureResult(createdRolePermission.Error, createdRolePermission.StatusCode);
            }

            return ServiceResult<string>.SuccessResult("User Role created sucessfully.", ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<EditUserRoleDTO>> GetUserRoleDetailsAsync(string id)
        {
            var result = await _userRolesRepository.GetRoleDetailsAsync(Guid.Parse(id));

            if (result.Data == null)
                return ServiceResult<EditUserRoleDTO>.FailureResult(result.Error, result.StatusCode);

            EditUserRoleDTO editUserRole = new()
            {
                Id = Guid.Parse(result.Data.Id),
                Name = result.Data.Name,
                NormalisedName = result.Data.NormalizedName,
                Description = result.Data.Description
            };

            var permissions = await _userRolesRepository.GetPermissionsForRoleAsync(id);

            if(permissions.Data.Count > 0)
            {
                foreach (var permission in permissions.Data)
                {
                    editUserRole.Permissions.Add(permission.PermissionId, permission.Permission.Name);
                }
            }

            return ServiceResult<EditUserRoleDTO>.SuccessResult(editUserRole);
        }

        public async Task<ServiceResult<EditUserRoleDTO>> UpdateRoleDetailsAsync(string id, EditUserRoleDTO userRole)
        {
            if (string.IsNullOrWhiteSpace(userRole.Name))
                return ServiceResult<EditUserRoleDTO>.FailureResult("Role name is required.", ServiceResultStatus.ValidationError);

            var roleResult = await _userRolesRepository.GetRoleDetailsAsync(Guid.Parse(id));

            if (!roleResult.Success)
                return ServiceResult<EditUserRoleDTO>.FailureResult(roleResult.Error!, roleResult.StatusCode);

            var role = roleResult.Data;

            if (role is null)
                return ServiceResult<EditUserRoleDTO>.FailureResult("Role not found.", ServiceResultStatus.NotFound);

            var incomingPermissionIds = userRole.Permissions.Keys.ToHashSet();

            var existingResult = await _userRolesRepository.GetPermissionsForRoleAsync(id);

            if (!existingResult.Success)
                return ServiceResult<EditUserRoleDTO>.FailureResult(existingResult.Error!, existingResult.StatusCode);

            var existingRolePermissions = existingResult.Data!;

            var existingPermissionIds = existingRolePermissions.Select(rp => rp.PermissionId).ToHashSet();

            var permissionsToRemove = existingRolePermissions.Where(rp => !incomingPermissionIds.Contains(rp.PermissionId)).ToList();

            var permissionIdsToAdd = incomingPermissionIds.Except(existingPermissionIds).ToList();

            if (permissionIdsToAdd.Count > 0)
            {
                foreach (var permissionId in permissionIdsToAdd)
                {
                    var permissionResult = await _permissionRepository.GetPermissionDetailsAsync(permissionId);

                    if (!permissionResult.Success)
                        return ServiceResult<EditUserRoleDTO>.FailureResult(permissionResult.Error!, permissionResult.StatusCode);

                    var permission = permissionResult.Data;

                    if (permission is null)
                        return ServiceResult<EditUserRoleDTO>.FailureResult($"Permission with ID '{permissionId}' does not exist.", ServiceResultStatus.ValidationError);

                    var newRolePermission = new RolePermission
                    {
                        RoleId = id,
                        PermissionId = permission.Id
                    };

                    var addResult = await _permissionRepository.CreatePermissionForRole(newRolePermission);

                    if (!addResult.Success)
                        return ServiceResult<EditUserRoleDTO>.FailureResult(addResult.Error!, addResult.StatusCode);
                }
            }

            foreach (var rolePermission in permissionsToRemove)
            {
                var removeResult = await _userRolesRepository.DeleteRolePermissionAsync(rolePermission);

                if (!removeResult.Success)
                    return ServiceResult<EditUserRoleDTO>.FailureResult(removeResult.Error!, removeResult.StatusCode);
            }

            role.Name = userRole.Name.Trim();
            role.Description = userRole.Description?.Trim();

            var updateResult = await _userRolesRepository.UpdateRoleAsync(role);

            if (!updateResult.Success)
                return ServiceResult<EditUserRoleDTO>.FailureResult(updateResult.Error!, updateResult.StatusCode);

            userRole.Name = role.Name;
            userRole.Description = role.Description;
            userRole.Permissions = incomingPermissionIds.ToDictionary(
                                    permissionId => permissionId,
                                    permissionId => existingRolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId)
                                    ?.Permission?.Name ?? string.Empty);

            return ServiceResult<EditUserRoleDTO>.SuccessResult(userRole);
        }

        public async Task<ServiceResult<string>> DeleteRoleAsync(string id, string currentUserRole)
        {
            if (string.IsNullOrWhiteSpace(currentUserRole))
                return ServiceResult<string>.FailureResult("Unable to determine your current role.", ServiceResultStatus.Forbidden);

            if (!Guid.TryParse(id, out var parsedRoleId))
                return ServiceResult<string>.FailureResult("Invalid Role ID.", ServiceResultStatus.ValidationError);

            var roleResult = await _userRolesRepository.GetRoleDetailsAsync(Guid.Parse(id));

            if (!roleResult.Success)
                return ServiceResult<string>.FailureResult(roleResult.Error!, roleResult.StatusCode);

            var role = roleResult.Data;

            if (role is null)
                return ServiceResult<string>.FailureResult("Role not found.", ServiceResultStatus.NotFound);

            if (string.Equals(currentUserRole, role.Name, StringComparison.OrdinalIgnoreCase))
                return ServiceResult<string>.FailureResult("You cannot delete the role assigned to your own account.", ServiceResultStatus.Forbidden);

            var currentUserRoleLevel = RoleHierarchy.RoleHierarchyDict.GetValueOrDefault(currentUserRole, 0);
            var targetRoleLevel = RoleHierarchy.RoleHierarchyDict.GetValueOrDefault(role.Name ?? string.Empty, 0);

            if (currentUserRoleLevel == 0 || targetRoleLevel == 0)
                return ServiceResult<string>.FailureResult("The role hierarchy is not configured correctly.", ServiceResultStatus.Forbidden);

            if (currentUserRoleLevel <= targetRoleLevel)
                return ServiceResult<string>.FailureResult("You are not allowed to delete this role.", ServiceResultStatus.Forbidden);

            var deleteResult = await _userRolesRepository.DeleteRoleAsync(role);

            if (!deleteResult.Success)
                return ServiceResult<string>.FailureResult(deleteResult.Error ?? "Failed to delete the role.", deleteResult.StatusCode);

            return ServiceResult<string>.SuccessResult("Role deleted successfully.", ServiceResultStatus.Success);
        }
    }
}