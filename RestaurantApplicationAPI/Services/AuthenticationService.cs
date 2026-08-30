using Microsoft.Extensions.Options;
using RestaurantApplicationAPI.Configuration;
using RestaurantApplicationAPI.DTO.Authentication;
using RestaurantApplicationAPI.RepositoryContracts;
using RestaurantApplicationAPI.ServiceContracts;

namespace RestaurantApplicationAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTTokenService _jwtTokenService;
        private readonly JWTSettings _jwtSettings;

        public AuthenticationService(IUserRepository userRepository, IJWTTokenService jwtTokenService, IOptions<JWTSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return null;

            var passwordValid = await _userRepository.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
                return null;

            var roles = await _userRepository.GetRoleByUsernameAsync(user);

            var accessToken = _jwtTokenService.GenerateToken(user, roles);

            return new LoginResponseDTO
                {
                    AccessToken = accessToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                    _jwtSettings.ExpirationMinutes)
                };
        }
    }
}