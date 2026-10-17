using Microsoft.EntityFrameworkCore;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDBContext _context;

        public PermissionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IList<Permission>>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _context.Permissions.OrderBy(x => x.Name).ToListAsync();

                return ServiceResult<IList<Permission>>.SuccessResult(permissions);
            }
            catch (Exception exception)
            {
                return ServiceResult<IList<Permission>>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<IEnumerable<string>> GetPermissionsForRolesAsync(IEnumerable<string> roles)
        {
            var permissionNames = await
                (from role in _context.Roles
                 join rolePermission in _context.RolePermissions
                 on role.Id equals rolePermission.RoleId
                 join permission in _context.Permissions
                 on rolePermission.PermissionId equals permission.Id
                 where roles.Contains(role.Name!)
                 select permission.Name)
                .Distinct().ToListAsync();

            return permissionNames;
        }

        public async Task<ServiceResult<Permission>> CreatePermissionAsync(Permission permission)
        {
            try
            {
                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                return ServiceResult<Permission>.SuccessResult(permission);
            }
            catch(Exception exception)
            {
                return ServiceResult<Permission>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }            
        }

        public async Task<ServiceResult<Permission>> GetPermissionDetailsAsync(string name)
        {
            try
            {
                Permission? permission = await _context.Permissions.FirstOrDefaultAsync(x => x.Name == name);

                if (permission == null)
                    return ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

                return ServiceResult<Permission>.SuccessResult(permission);
            }
            catch (Exception exception)
            {
                return ServiceResult<Permission>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<Permission>> GetPermissionDetailsAsync(Guid Id)
        {
            try
            {
                Permission? permission = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == Id);

                if (permission == null)
                    return ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

                return ServiceResult<Permission>.SuccessResult(permission);
            }
            catch (Exception exception)
            {
                return ServiceResult<Permission>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<Permission>> UpdatePermissionAsync(Permission permission)
        {
            try
            {
                var existingPermission = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == permission.Id);

                if (existingPermission == null)
                    return ServiceResult<Permission>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

                var isDuplicateName = await _context.Permissions.AnyAsync(x => x.Name == permission.Name && x.Id != permission.Id);

                if (isDuplicateName)
                    return ServiceResult<Permission>.FailureResult("A permission with this name already exists.", ServiceResultStatus.Conflict);

                existingPermission.Name = permission.Name;
                existingPermission.Description = permission.Description;

                await _context.SaveChangesAsync();

                return ServiceResult<Permission>.SuccessResult(existingPermission);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<Permission>.FailureResult("Failed to update permission", ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<bool>> DeletePermissionAsync(Guid Id)
        {
            try
            {
                var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == Id);

                if (permission == null)
                    return ServiceResult<bool>.FailureResult("Permission not found", ServiceResultStatus.NotFound);

                var rolePermissions = await _context.RolePermissions.Where(rp => rp.PermissionId == Id).ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermissions);
                _context.Permissions.Remove(permission);

                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.FailureResult("Failed to Delete permission", ServiceResultStatus.Error);
            }
        }
    }
}