using Microsoft.AspNetCore.Identity;

namespace RestaurantApplicationAPI.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}