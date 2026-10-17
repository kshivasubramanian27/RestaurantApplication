using System.Net;

namespace RestaurantApplicationUI.Exceptions
{
    public class ApiRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ApiRequestException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}