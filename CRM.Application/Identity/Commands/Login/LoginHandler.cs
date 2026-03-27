

using CRM.Application.Identity.DTOs.Auth;

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginHandler:ILoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponseDto> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null ||
                !_passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials");
            }

            if (user.IsDisabled())
                throw new Exception("User is disabled");

            var accessToken = _jwtService.GenerateToken(user, user.TenantId);
            var refreshToken = await _refreshTokenService.GenerateAsync(user, user.TenantId);

            return new AuthResponseDto
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

    }
}
