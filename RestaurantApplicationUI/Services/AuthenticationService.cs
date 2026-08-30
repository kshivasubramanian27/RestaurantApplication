using RestaurantApplicationUI.DTO.Authentication;
using RestaurantApplicationUI.ServiceContracts;

namespace RestaurantApplicationUI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RestaurantAPI");
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Authentication/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
        }
    }
}