using RestaurantApplicationAPI.DTO.Users;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;
using RestaurantApplicationAPI.Utilities;

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

        public async Task<(bool Success, string Error)> CreateUserAsync(CreateUserRequestDTO request, string creatorUsername)
        {
            var creatingUser = await _userRepository.GetUserByUsernameAsync(creatorUsername);

            if (creatingUser == null)
                return (false, "Creating User account was not found.");

            var creatingUserRole = await _userRepository.GetRoleByUsernameAsync(creatingUser);

            var creatorLevel = creatingUserRole
                .Where(role => RoleHierarchy.RoleHierarchyDict.ContainsKey(role))
                .Select(role => RoleHierarchy.RoleHierarchyDict[role])
                .DefaultIfEmpty(0).Max();

            if (creatorLevel == 0)
                return (false, "You are not authorized to create users.");

            if (!RoleHierarchy.RoleHierarchyDict.TryGetValue(request.RoleName, out var requestedLevel))
                return (false, "The selected role is invalid.");

            if (requestedLevel > creatorLevel)
                return (false, "You cannot create a user with a higher-level role.");

            var newUser = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userRepository.CreateUserAsync(newUser, request.Password, request.RoleName);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));

                return (false, errors);
            }

            return (true, string.Empty);
        }
    }
}