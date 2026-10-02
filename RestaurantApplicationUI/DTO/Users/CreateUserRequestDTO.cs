using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantApplicationUI.DTO.Users
{
    public class CreateUserRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string? LastName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [JsonIgnore]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}