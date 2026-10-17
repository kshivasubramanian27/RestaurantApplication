namespace RestaurantApplicationUI.DTO.Common
{
    public class ApiErrorResponse
    {
        public string? Message { get; set; }

        public string? Title { get; set; }

        public string? Detail { get; set; }

        public int? Status { get; set; }
    }
}