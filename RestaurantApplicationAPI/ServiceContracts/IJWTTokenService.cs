using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.ServiceContracts
{
    public interface IJWTTokenService
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}