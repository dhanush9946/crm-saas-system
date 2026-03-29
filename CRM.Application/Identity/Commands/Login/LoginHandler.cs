

//using CRM.Application.Common.Exceptions;
//using CRM.Application.Identity.DTOs.Auth;
//using CRM.Application.Identity.Interfaces;
//using CRM.Application.Common.Interfaces;

//namespace CRM.Application.Identity.Commands.Login
//{
//    public class LoginHandler:ILoginHandler
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IPasswordHasher _passwordHasher;
//        private readonly IJwtService _jwtService;
//        private readonly IRefreshTokenService _refreshTokenService;
//        private readonly ITenantRepository _tenantRepository;

//        public LoginHandler(
//        IUserRepository userRepository,
//        IPasswordHasher passwordHasher,
//        IJwtService jwtService,
//        IRefreshTokenService refreshTokenService,
//        ITenantRepository tenantRepository)
//        {
//            _userRepository = userRepository;
//            _passwordHasher = passwordHasher;
//            _jwtService = jwtService;
//            _refreshTokenService = refreshTokenService;
//            _tenantRepository = tenantRepository;
//        }

//        public async Task<AuthResponseDto> HandleAsync(LoginCommand command)
//        {
//            var tenant = await _tenantRepository.GetBySlugAsync(command.TenantSlug);

//            if (tenant == null)
//                throw new UnauthorizedException("Invalid tenant");

//            var user = await _userRepository.GetByEmailAsync(tenant.Id,command.Email);

//            if (user == null || user.PasswordHash == null ||
//               !_passwordHasher.Verify(command.Password, user.PasswordHash))
//            {
//                throw new UnauthorizedException("Invalid credentials");
//            }



//            if (user.IsDisabled())
//                throw new Exception("User is disabled");

//            var accessToken = _jwtService.GenerateToken(user.Id, user.TenantId,user.Email);
//            var refreshToken = await _refreshTokenService.GenerateAsync(user, user.TenantId);

//            return new AuthResponseDto
//            {
//                TenantId = user.TenantId,
//                UserId = user.Id,
//                AccessToken = accessToken,
//                RefreshToken = refreshToken
//            };
//        }

//    }
//}


using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginHandler : ILoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITenantRepository _tenantRepository;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            ITenantRepository tenantRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _tenantRepository = tenantRepository;
        }

        public async Task<AuthResponseDto> HandleAsync(LoginCommand command)
        {
            // 1. Get tenant
            var tenant = await _tenantRepository.GetBySlugAsync(command.TenantSlug);

            if (tenant == null)
                throw new UnauthorizedException("Invalid tenant");

            // 2. Get user
            var user = await _userRepository.GetByEmailAsync(tenant.Id, command.Email);

            if (user == null || user.PasswordHash == null ||
                !_passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // 3. Check disabled
            if (user.IsDisabled())
                throw new UnauthorizedException("User is disabled");

            // 4. Generate tokens 
            var (accessToken, refreshToken) =
                await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    user.Email,
                    command.DeviceId,
                    null,
                    null
                );

            // 5. Response
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
