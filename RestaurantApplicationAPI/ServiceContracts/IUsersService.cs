using RestaurantApplicationAPI.DTO.Users;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsers();

        public Task<(bool Success, string Error)> CreateUserAsync(CreateUserRequestDTO request, string creatorUsername);

        public Task<UsersDTO> GetUserByIdAsync(string userId);
    }
}