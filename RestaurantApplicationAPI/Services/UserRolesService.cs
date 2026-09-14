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
                            RoleName = role.Name,
                            RoleDescrption = role.Description,
                            NormalisedName = role.NormalizedName
                        }
                    );
            }

            return userRoles;
        }
    }
}