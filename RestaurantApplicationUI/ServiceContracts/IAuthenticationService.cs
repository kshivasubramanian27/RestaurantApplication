using RestaurantApplicationUI.DTO.Authentication;

namespace RestaurantApplicationUI.ServiceContracts
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request);
    }
}