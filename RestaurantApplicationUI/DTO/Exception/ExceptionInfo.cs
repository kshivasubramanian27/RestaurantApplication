namespace RestaurantApplicationUI.DTO.Exception
{
    public struct ExceptionInfo
    {
        public string ControllerName { get; set; }
        public string StackTrace { get; set; }

        public ExceptionInfo(string controllerName, string stackTrace)
        {
            ControllerName = controllerName;
            StackTrace = stackTrace;
        }
    }
}