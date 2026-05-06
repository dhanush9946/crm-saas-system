using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Domain.Identity.Constants;
using MediatR;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public RegisterUserHandler(
            IUserRepository userRepository,
            ITenantRepository tenantRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Check tenant already exists
            var existingTenant = await _tenantRepository
                .GetBySlugAsync(request.TenantSlug, cancellationToken);

            if (existingTenant != null)
                throw new ConflictException("Tenant already exists");

            // 2. Create Tenant
            var tenant = Tenant.Create(request.TenantName, request.TenantSlug);

            // 3. Create User
            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = User.Create(
                tenant.Id,
                request.Email,
                request.DisplayName
            );

            user.SetPasswordHash(passwordHash);

            // 4. Ensure OWNER role exists
            const string roleName = RoleConstants.Owner;
            var normalized = roleName.ToUpperInvariant();

            var ownerRole = await _roleRepository
                .GetByNameAsync(tenant.Id, normalized);

            if (ownerRole == null)
            {
                ownerRole = Role.Create(tenant.Id, roleName, true);
                await _roleRepository.AddAsync(ownerRole, cancellationToken);
            }

            // 5. Assign role to user
            var exists = await _userRoleRepository
                .ExistsAsync(tenant.Id, user.Id, ownerRole.Id);

            if (!exists)
            {
                var userRole = UserRole.Create(
                    tenant.Id,
                    user.Id,
                    ownerRole.Id
                );

                await _userRoleRepository.AddAsync(userRole, cancellationToken);
            }

            // 6. Add entities (NO SAVE YET)
            await _tenantRepository.AddAsync(tenant, cancellationToken);
            await _userRepository.AddAsync(user, cancellationToken);


            // 8. Generate tokens
            var roles = new List<string> { roleName };
            var accessToken = _jwtService.GenerateToken(user.Id, user.TenantId, user.Email, user.TokenVersion, roles);

            var refreshToken = await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    null,
                    null,
                    null,
                    cancellationToken
                );

            // 9. SINGLE SAVE ALL
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 10. Return response
            return new AuthResponseDto
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
