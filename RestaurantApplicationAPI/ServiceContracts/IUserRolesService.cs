using RestaurantApplicationAPI.DTO.Roles;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IUserRolesService
    {
        public Task<IList<UserRolesDTO>> GetAllRolesAsync();

        public Task<IList<UserPermissionsDTO>> GetAllPermissionsAsync();
    }
}