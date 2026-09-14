namespace RestaurantApplicationAPI.DTO.Users
{
    public class CreateUserRequestDTO
    {
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}