using System.Security.Claims;

namespace RestaurantApplicationUI.Extensions
{
    public static class PermissionClaimsExtension
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.HasClaim("permission", permission);
        }
    }
}