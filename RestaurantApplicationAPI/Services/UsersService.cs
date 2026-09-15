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
                    Id = user.Id ?? string.Empty,
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

        public async Task<UsersDTO> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userRole = await _userRepository.GetRoleByUsernameAsync(user);

            return new UsersDTO
            {
                Id = user.Id ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
                IsActive = user.IsActive,
                Roles = userRole
            };
        }

        public async Task<(bool Success, string Error)> UpdateUserAsync(UpdateUserDTO request, string currentUserId)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(currentUserId);

            if (currentUser == null)
                return (false, "The current user account was not found.");

            var targetUser = await _userRepository.GetUserByIdAsync(request.Id);

            if (targetUser == null)
                return (false, "The user you are trying to update was not found.");

            var currentUserRoles = await _userRepository.GetRoleByUsernameAsync(currentUser);

            var currentUserLevel = currentUserRoles.Where(role => RoleHierarchy.RoleHierarchyDict.ContainsKey(role))
                                                    .Select(role => RoleHierarchy.RoleHierarchyDict[role])
                                                    .DefaultIfEmpty(0).FirstOrDefault();

            if (currentUserLevel == 0)
                return (false, "You are not authorized to update users.");

            var targetUserRoles = await _userRepository.GetRoleByUsernameAsync(targetUser);

            var targetUserLevel = targetUserRoles.Where(role => RoleHierarchy.RoleHierarchyDict.ContainsKey(role))
                                                    .Select(role => RoleHierarchy.RoleHierarchyDict[role])
                                                    .DefaultIfEmpty(0).FirstOrDefault();

            if (targetUserLevel > currentUserLevel)
                return (false, "You cannot edit a user with a higher-level role than your own.");

            var isEditingOwnAccount = string.Equals(currentUser.Id, targetUser.Id, StringComparison.OrdinalIgnoreCase);

            if (!RoleHierarchy.RoleHierarchyDict.TryGetValue(request.RoleName, out var requestedRoleLevel))
                return (false, "The selected role is invalid.");

            if (requestedRoleLevel > currentUserLevel)
                return (false, "You cannot assign a role higher than your own.");

            var currentRole = targetUserRoles.FirstOrDefault();

            if (isEditingOwnAccount && !string.Equals(currentRole, request.RoleName, StringComparison.OrdinalIgnoreCase))
                return (false, "You cannot change your own role.");

            targetUser.FirstName = request.FirstName;
            targetUser.LastName = request.LastName;
            targetUser.Email = request.Email;
            targetUser.PhoneNumber = request.PhoneNumber;
            targetUser.Address = request.Address;
            targetUser.IsActive = request.IsActive;

            var updateResult = await _userRepository.UpdateUserAsync(targetUser);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(error => error.Description));

                return (false, errors);
            }

            if (!string.Equals(currentRole, request.RoleName, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(currentRole))
                {
                    var removeResult = await _userRepository.RemoveUserFromRoleAsync(targetUser, currentRole);

                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join("; ", removeResult.Errors.Select(error => error.Description));

                        return (false, errors);
                    }
                }

                var addResult = await _userRepository.AddUserToRoleAsync(targetUser, request.RoleName);

                if (!addResult.Succeeded)
                {
                    var errors = string.Join("; ", addResult.Errors.Select(error => error.Description));

                    return (false, errors);
                }
            }

            return (true, string.Empty);
        }
    }
}