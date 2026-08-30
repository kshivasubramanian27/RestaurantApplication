using Azure.Core;
using Microsoft.AspNetCore.Identity;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;

namespace RestaurantApplicationAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<ApplicationUser?> GetUserByUsernameAsync(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<IList<string>> GetRoleByUsernameAsync(ApplicationUser user)
        {
            return _userManager.GetRolesAsync(user);
        }
    }
}