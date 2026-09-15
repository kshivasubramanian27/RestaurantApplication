using RestaurantApplicationUI.DTO.Common;
using RestaurantApplicationUI.DTO.Roles;
using RestaurantApplicationUI.DTO.Users;
using RestaurantApplicationUI.ServiceContracts;
using System.Net.Http.Headers;

namespace RestaurantApplicationUI.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpClient _httpClient;

        public UsersService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RestaurantAPI");
        }

        public async Task<IList<UsersDTO>> GetAllUsersAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("api/Users");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<IList<UsersDTO>>();

            return users;
        }

        public async Task<IList<UserRolesDTO>> GetAllRolesAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("api/UserRoles");

            response.EnsureSuccessStatusCode();

            var roles = await response.Content.ReadFromJsonAsync<IList<UserRolesDTO>>();

            return roles ?? new List<UserRolesDTO>();
        }

        public async Task<(bool success, string? error)> CreateUserAsync(CreateUserRequestDTO request, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsJsonAsync("api/Users", request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

            return (false, error?.Message ?? "Unable to create the user.");
        }

        public async Task<UsersDTO> GetUserByIdAsync(string accessToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"api/Users/{userId}");

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UsersDTO>();

            return user;
        }
    }
}