using System.ComponentModel.DataAnnotations;

namespace RestaurantApplicationAPI.DTO.Users
{
    public class UpdateUserDTO
    {
        public string Id { get; set; }

        [Required]
        public string? FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}