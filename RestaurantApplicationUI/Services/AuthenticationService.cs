using RestaurantApplicationUI.DTO.Authentication;
using RestaurantApplicationUI.ServiceContracts;
using System.Net.Http.Headers;

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

        public async Task<bool> LogoutAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync("api/Authentication/logout", content: null);

            return response.IsSuccessStatusCode;
        }
    }
}