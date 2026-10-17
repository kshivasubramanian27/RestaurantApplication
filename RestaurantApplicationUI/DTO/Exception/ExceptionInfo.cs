namespace RestaurantApplicationUI.DTO.Exception
{
    public struct ExceptionInfo
    {
        public string? ControllerName { get; set; }

        public string? ErrorMessage { get; set; }

        public int StatusCode { get; set; }

        public string? StackTrace { get; set; }

        public ExceptionInfo(string controllerName, string errorMessage, int statusCode, string stackTrace)
        {
            ControllerName = controllerName;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            StackTrace = stackTrace;
        }
    }
}