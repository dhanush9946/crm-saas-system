
using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand,AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            ITenantRepository tenantRepository,
            IUserRoleRepository userRoleRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _tenantRepository = tenantRepository;
            _userRoleRepository = userRoleRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request,CancellationToken cancellationToken)
        {
            // 1. Get tenant
            var tenant = await _tenantRepository.GetBySlugAsync(request.TenantSlug,cancellationToken);

            if (tenant == null)
                throw new UnauthorizedException("Invalid tenant");

            // 2. Get user
            var user = await _userRepository.GetByEmailAsync(tenant.Id, request.Email,cancellationToken);

            if (user == null || user.PasswordHash == null ||
                !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // 3. Check disabled
            if (user.IsDisabled())
                throw new UnauthorizedException("User is disabled");

            user.RecordLogin();

            // 4. Fetch roles
            var roles = await _userRoleRepository.GetUserRolesAsync(user.TenantId, user.Id, cancellationToken);

            // 5. Generate tokens 
            var accessToken = _jwtService.GenerateToken(user.Id, user.TenantId, user.Email, user.TokenVersion, roles);
            
            var refreshToken = await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    request.DeviceId,
                    null,
                    null,
                    cancellationToken
                );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
