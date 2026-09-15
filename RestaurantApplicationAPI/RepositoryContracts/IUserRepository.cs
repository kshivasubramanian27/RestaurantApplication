using Microsoft.AspNetCore.Identity;
using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IUserRepository
    {
        public Task<ApplicationUser?> GetUserByUsernameAsync(string username);

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        public Task<IList<string>> GetRoleByUsernameAsync(ApplicationUser user);

        public Task<IList<ApplicationUser>> GetAllUsersAsync();

        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string roleName);

        public Task<ApplicationUser?> GetUserByIdAsync(string userId);

        public Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        public Task<IdentityResult> RemoveUserFromRoleAsync(ApplicationUser user, string roleName);

        public Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName);
    }
}