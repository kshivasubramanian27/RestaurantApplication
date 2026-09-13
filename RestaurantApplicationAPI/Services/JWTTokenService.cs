using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestaurantApplicationAPI.Configuration;
using RestaurantApplicationAPI.Models;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantApplicationAPI.Services
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly JWTSettings _jwtSettings;

        private readonly IPermissionRepository _permissionRepository;

        public JWTTokenService(IOptions<JWTSettings> jwtSettings, IPermissionRepository permissionRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _permissionRepository = permissionRepository;
        }

        public async Task<string> GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var permissions = await _permissionRepository.GetPermissionsForRolesAsync(roles);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName ?? string.Empty),

                new Claim(
                    ClaimTypes.GivenName,
                    user.FirstName ?? string.Empty),

                new Claim(
                    ClaimTypes.Surname,
                    user.LastName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim(
                    "permission",
                    permission));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}