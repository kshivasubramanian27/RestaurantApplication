using RestaurantApplicationUI.DTO.Roles;
using RestaurantApplicationUI.DTO.Users;

namespace RestaurantApplicationUI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsersAsync(string accessToken);

        Task<IList<UserRolesDTO>> GetAllRolesAsync(string accessToken);

        Task<(bool success, string? error)> CreateUserAsync(CreateUserRequestDTO request, string accessToken);
    }
}