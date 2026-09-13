using Microsoft.EntityFrameworkCore;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.RepositoryContracts;

namespace RestaurantApplicationAPI.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDBContext _context;

        public PermissionRepository(ApplicationDBContext context)
        {
            _context = context;
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
    }
}