using RestaurantApplicationUI.DTO.Authentication;

namespace RestaurantApplicationUI.ServiceContracts
{
    public interface IAuthenticationService
    {
        public Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request);

        public Task<bool> LogoutAsync(string accessToken);
    }
}