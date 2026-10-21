namespace RestaurantApplicationAPI.Utilities
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }

        public T? Data { get; set; }

        public string? Error { get; set; }

        public ServiceResultStatus StatusCode { get; set; }

        public static ServiceResult<T> SuccessResult(T data, ServiceResultStatus statusCode = ServiceResultStatus.Success)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ServiceResult<T> FailureResult(string error, ServiceResultStatus status)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Error = error,
                StatusCode = status
            };
        }
    }
}