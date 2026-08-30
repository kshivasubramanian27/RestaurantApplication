using RestaurantApplicationAPI.DTO.Authentication;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDTO>? LoginAsync(LoginRequestDTO request);
    }
}