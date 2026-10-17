using Microsoft.AspNetCore.Identity;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

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
    }
}