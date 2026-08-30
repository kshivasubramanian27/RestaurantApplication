using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<ApplicationUser>? GetUserByUsernameAsync(string username);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        public Task<IList<string>> GetRoleByUsernameAsync(ApplicationUser user);
    }
}