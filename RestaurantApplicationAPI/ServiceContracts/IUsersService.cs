using RestaurantApplicationAPI.DTO.Users;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsers();
    }
}