
using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using MediatR;

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand,AuthResponseDto>
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

        public async Task<AuthResponseDto> Handle(LoginCommand reuquest,CancellationToken cancellationToken)
        {
            // 1. Get tenant
            var tenant = await _tenantRepository.GetBySlugAsync(reuquest.TenantSlug,cancellationToken);

            if (tenant == null)
                throw new UnauthorizedException("Invalid tenant");

            // 2. Get user
            var user = await _userRepository.GetByEmailAsync(tenant.Id, reuquest.Email,cancellationToken);

            if (user == null || user.PasswordHash == null ||
                !_passwordHasher.Verify(reuquest.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // 3. Check disabled
            if (user.IsDisabled())
                throw new UnauthorizedException("User is disabled");

            user.RecordLogin();
            await _userRepository.SaveChangesAsync(cancellationToken);

            // 4. Generate tokens 
            var (accessToken, refreshToken) =
                await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    user.Email,
                    reuquest.DeviceId,
                    null,
                    null,
                    cancellationToken
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
