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
    }
}