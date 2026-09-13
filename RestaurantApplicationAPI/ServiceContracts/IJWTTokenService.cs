using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IJWTTokenService
    {
        Task<string> GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}