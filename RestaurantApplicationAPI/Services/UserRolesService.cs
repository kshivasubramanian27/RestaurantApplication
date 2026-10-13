using RestaurantApplicationAPI.DTO.Roles;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Services
{
    public class UserRolesService : IUserRolesService
    {
        private readonly IUserRolesRepository _userRolesRepository;

        public UserRolesService(IUserRolesRepository userRolesRepository)
        {
            _userRolesRepository = userRolesRepository;
        }

        public async Task<IList<UserRolesDTO>> GetAllRolesAsync()
        {
            var allRoles = await _userRolesRepository.GetAllRolesAsync();

            List<UserRolesDTO> userRoles = new();

            foreach (var role in allRoles)
            {
                userRoles.Add(
                    new UserRolesDTO
                        {
                            Id = role.Id,
                            RoleName = role.Name,
                            RoleDescrption = role.Description,
                            NormalisedName = role.NormalizedName
                        }
                    );
            }

            return userRoles;
        }

        public async Task<IList<UserPermissionsDTO>> GetAllPermissionsAsync()
        {
            var allPermissions = await _userRolesRepository.GetAllPermissionsAsync();

            List<UserPermissionsDTO> permissions = new();

            foreach(var permission in allPermissions)
            {
                permissions.Add(new UserPermissionsDTO
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                });
            }

            return permissions;
        }
    }
}