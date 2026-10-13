using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.RepositoryContracts
{
    public interface IUserRolesRepository
    {
        public Task<IList<ApplicationRole>> GetAllRolesAsync();

        public Task<IList<Permission>> GetAllPermissionsAsync();
    }
}