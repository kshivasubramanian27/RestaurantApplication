using RestaurantApplicationUI.DTO.Users;

namespace RestaurantApplicationUI.ServiceContracts
{
    public interface IUsersService
    {
        public Task<IList<UsersDTO>> GetAllUsersAsync(string accessToken);
    }
}