using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.Utilities;
using System.Data;

namespace RestaurantApplicationAPI.Repository
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDBContext _dbContext;

        public UserRolesRepository(RoleManager<ApplicationRole> roleManager, ApplicationDBContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public Task<IList<ApplicationRole>> GetAllRolesAsync()
        {
            var allRoles = Task.FromResult<IList<ApplicationRole>>(_roleManager.Roles.ToList());

            return allRoles;
        }

        public async Task<ServiceResult<ApplicationRole>> GetRoleDetailsAsync(string roleName)
        {
            try
            {
                var roleDetails = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleName);

                if(roleDetails != null)
                    return ServiceResult<ApplicationRole>.FailureResult("Role already exists", ServiceResultStatus.Conflict);

                return ServiceResult<ApplicationRole>.SuccessResult(roleDetails);
            }
            catch (Exception exception)
            {
                return ServiceResult<ApplicationRole>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<ApplicationRole>> GetRoleDetailsAsync(Guid id)
        {
            try
            {
                var roleDetails = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id.ToString());

                if (roleDetails == null)
                    return ServiceResult<ApplicationRole>.FailureResult("Role does not exist", ServiceResultStatus.NotFound);

                return ServiceResult<ApplicationRole>.SuccessResult(roleDetails);
            }
            catch (Exception exception)
            {
                return ServiceResult<ApplicationRole>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<IdentityResult>> CreateRoleAsync(ApplicationRole newRole)
        {
            try
            {
                var result = await _roleManager.CreateAsync(newRole);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));

                    return ServiceResult<IdentityResult>.FailureResult(errors, ServiceResultStatus.ValidationError);
                }

                return ServiceResult<IdentityResult>.SuccessResult(result);
            }
            catch (Exception exception)
            {
                return ServiceResult<IdentityResult>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<IList<RolePermission>>> GetPermissionsForRoleAsync(string roleId)
        {
            try
            {
                var result = await _dbContext.RolePermissions.Where(rp => rp.RoleId == roleId).Include(rp => rp.Permission).ToListAsync();

                return ServiceResult<IList<RolePermission>>.SuccessResult(result);
            }
            catch (Exception exception)
            {
                return ServiceResult<IList<RolePermission>>.FailureResult(exception.ToString(), ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<ApplicationRole>> UpdateRoleAsync(ApplicationRole role)
        {
            try
            {
                _dbContext.Roles.Update(role);

                await _dbContext.SaveChangesAsync();

                return ServiceResult<ApplicationRole>.SuccessResult(role);
            }
            catch (Exception exception)
            {
                return ServiceResult<ApplicationRole>.FailureResult("An error occurred while updating the role.", ServiceResultStatus.Error);
            }
        }

        public async Task<ServiceResult<bool>> DeleteRolePermissionAsync(RolePermission rolePermission)
        {
            try
            {
                _dbContext.RolePermissions.Remove(rolePermission);

                await _dbContext.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception exception)
            {
                return ServiceResult<bool>.FailureResult("Failed to remove the permission assignment from the role.", ServiceResultStatus.Error);
            }
        }
    }
}