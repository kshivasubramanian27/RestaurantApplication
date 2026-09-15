using RestaurantApplicationAPI.DTO.Users;
using RestaurantApplicationAPI.Utilities;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsers();

        public Task<(bool Success, string Error)> CreateUserAsync(CreateUserRequestDTO request, string creatorUsername);

        public Task<UsersDTO> GetUserByIdAsync(string userId);

        public Task<(bool Success, string Error)> UpdateUserAsync(UpdateUserDTO request, string currentUserId);
    }
}