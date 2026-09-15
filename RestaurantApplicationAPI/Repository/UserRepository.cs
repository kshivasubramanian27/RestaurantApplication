using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IList<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.AsNoTracking().ToListAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string roleName)
        {
            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return createResult;

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return roleResult;
            }

            return IdentityResult.Success;
        }

        public Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }
    }
}