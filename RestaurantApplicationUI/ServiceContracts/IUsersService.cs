using RestaurantApplicationUI.DTO.Roles;
using RestaurantApplicationUI.DTO.Users;

namespace RestaurantApplicationUI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsersAsync(string accessToken);

        Task<IList<UserRolesDTO>> GetAllRolesAsync(string accessToken);

        Task<(bool success, string? error)> CreateUserAsync(CreateUserRequestDTO request, string accessToken);

        Task<UsersDTO> GetUserByIdAsync(string accessToken, string userId);

        Task<(bool success, string? error)> UpdateUserAsync(string accessToken, UpdateUserDTO model);

        Task<(bool success, string? error)> DeleteUserAsync(string accessoken, string userId);

        Task<IList<UserPermissionsDTO>> GetAllPermissionsAsync(string accessToken);

        Task<(bool success, string? error)> CreatePermissionAsync(string accessToken, CreateUserPermissionDTO model);

        Task<EditUserPermissionDTO> GetPermissionDetailsByIdAsync(string accessToken, string Id);

        Task<(bool success, string? error)> UpdatePermissionDetailsAsync(string accessToken, EditUserPermissionDTO model);

        Task<(bool success, string? error)> DeletePermissionAsync(string accessToken, Guid Id);
    }
}