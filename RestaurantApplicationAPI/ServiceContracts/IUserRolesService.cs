using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IUserRolesService
    {
        public Task<IList<UserRolesDTO>> GetAllRolesAsync();

        public Task<ServiceResult<string>> CreateUserRoleAsync(CreateUserRoleDTO userRole);

        public Task<ServiceResult<EditUserRoleDTO>> GetUserRoleDetailsAsync(string id);

        public Task<ServiceResult<EditUserRoleDTO>> UpdateRoleDetailsAsync(string id, EditUserRoleDTO userRole);

        public Task<ServiceResult<string>> DeleteRoleAsync(string roleId, string currentUserId);
    }
}