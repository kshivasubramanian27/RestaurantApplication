using Microsoft.AspNetCore.Identity;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;

namespace RestaurantApplicationAPI.Repository
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserRolesRepository(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<IList<ApplicationRole>> GetAllRolesAsync()
        {
            var allRoles = Task.FromResult<IList<ApplicationRole>>(_roleManager.Roles.ToList());

            return allRoles;
        }
    }
}