using RestaurantApplicationUI.DTO.Roles;
using RestaurantApplicationUI.DTO.Users;
using RestaurantApplicationUI.Exceptions;
using RestaurantApplicationUI.ServiceContracts;
using RestaurantApplicationUI.Utilities;
using System.Net;
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

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve users.");

            var users = await response.Content.ReadFromJsonAsync<IList<UsersDTO>>();

            return users ?? new List<UsersDTO>();
        }

        public async Task<IList<UserRolesDTO>> GetAllRolesAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("api/UserRoles");

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve roles.");

            var roles = await response.Content.ReadFromJsonAsync<IList<UserRolesDTO>>();

            return roles ?? new List<UserRolesDTO>();
        }

        public async Task<(bool success, string? error)> CreateUserAsync(CreateUserRequestDTO request, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsJsonAsync("api/Users", request);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to create the user."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<UsersDTO> GetUserByIdAsync(string accessToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"api/Users/{userId}");

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve the user.");

            var user = await response.Content.ReadFromJsonAsync<UsersDTO>();

            return user ?? new();
        }

        public async Task<(bool success, string? error)> UpdateUserAsync(string accessToken, UpdateUserDTO model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PutAsJsonAsync($"api/Users/{model.Id}", model);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to update the user."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<(bool success, string? error)> DeleteUserAsync(string accessToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.DeleteAsync($"api/Users/{userId}");

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to delete the user."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<IList<UserPermissionsDTO>> GetAllPermissionsAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"api/UserRoles/GetAllPermissions");

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve permissions.");

            var permissions = await response.Content.ReadFromJsonAsync<IList<UserPermissionsDTO>>();

            return permissions ?? new List<UserPermissionsDTO>();
        }

        public async Task<(bool success, string? error)> CreatePermissionAsync(string accessToken, CreateUserPermissionDTO model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsJsonAsync("api/UserRoles/AddPermission", model);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to create the new user permission."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<EditUserPermissionDTO> GetPermissionDetailsByIdAsync(string accessToken, string Id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"api/UserRoles/EditPermission/{Id}");

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve permission details.");

            var permissionDetails = await response.Content.ReadFromJsonAsync<EditUserPermissionDTO>();

            return permissionDetails ?? new EditUserPermissionDTO();
        }

        public async Task<(bool success, string? error)> UpdatePermissionDetailsAsync(string accessToken, EditUserPermissionDTO model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PutAsJsonAsync($"api/UserRoles/EditPermission/{model.Id}", model);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to update the permission."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<(bool success, string? error)> DeletePermissionAsync(string accessToken, Guid id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.DeleteAsync($"api/UserRoles/DeletePermission/{id}");

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to delete the permission."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<(bool success, string? error)> CreateRoleAsync(string accessToken, CreateUserRoleDTO model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            model.NormalisedName = model.Name.ToUpper();

            var response = await _httpClient.PostAsJsonAsync("api/UserRoles/AddRole", model);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to create the new user role."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<EditUserRoleDTO> GetRoleDetailsAsync(string accessToken, Guid Id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"api/UserRoles/EditRole/{Id}");

            await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to retrieve Role details.");

            var roleDetails = await response.Content.ReadFromJsonAsync<EditUserRoleDTO>();

            return roleDetails ?? new EditUserRoleDTO();
        }

        public async Task<(bool success, string? error)> UpdatePermissionDetailsAsync(string accessToken, EditUserRoleDTO model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PutAsJsonAsync($"api/UserRoles/EditRole/{model.Id}", model);

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to update the Role."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteRoleAsync(string accessToken, string roleId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.DeleteAsync($"api/UserRoles/{roleId}");

            try
            { await ApiErrorHandler.EnsureSuccessAsync(response, "Unable to delete the role."); }
            catch (ApiRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict || exception.StatusCode == HttpStatusCode.BadRequest)
            { return (false, exception.Message); }

            return (true, null);
        }
    }
}