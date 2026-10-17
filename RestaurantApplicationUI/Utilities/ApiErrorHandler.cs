using RestaurantApplicationUI.DTO.Common;
using RestaurantApplicationUI.Exceptions;
using System.Text.Json;

namespace RestaurantApplicationUI.Utilities
{
    public static class ApiErrorHandler
    {
        public static async Task EnsureSuccessAsync(HttpResponseMessage response, string fallbackMessage)
        {
            if (response.IsSuccessStatusCode)
                return;

            string? message = null;

            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

                message = error?.Message;

                if (string.IsNullOrWhiteSpace(message))
                    message = error?.Detail;

                if (string.IsNullOrWhiteSpace(message))
                    message = error?.Title;
            }
            catch (JsonException)
            {
                // The response body was not valid JSON.
            }

            if (string.IsNullOrWhiteSpace(message))
                message = fallbackMessage;

            throw new ApiRequestException(response.StatusCode, message);
        }
    }
}