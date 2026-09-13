using RestaurantApplicationAPI.DTO.Users;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;

        public UsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IList<UsersDTO>> GetAllUsers()
        {
            var allUsers = await _userRepository.GetAllUsersAsync();

            var usersDTO = new List<UsersDTO>();

            foreach (var user in allUsers)
            {
                var roles = await _userRepository.GetRoleByUsernameAsync(user);

                usersDTO.Add(new UsersDTO
                {
                    Username = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    IsActive = user.IsActive,
                    Roles = roles
                });
            }

            return usersDTO;
        }
    }
}