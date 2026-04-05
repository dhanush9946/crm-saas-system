using CRM.Application.Identity.DTOs.Auth;
using CRM.Domain.Identity.Entities;
using CRM.Application.Identity.Validators;
using CRM.Application.Common.Exceptions;
using FluentValidation;
using CRM.Application.Identity.Interfaces;
using MediatR;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserHandler:IRequestHandler<RegisterUserCommand,AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public RegisterUserHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request,CancellationToken cancellationToken)
        {
            var validator = new RegisterUserValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            //checks tenant already existed
            var existingTenant = await _tenantRepository
            .GetBySlugAsync(request.TenantSlug,cancellationToken);

            if (existingTenant != null)
                throw new ConflictException("Tenant already exists");

            //creating Tenant
            var tenant = Tenant.Create(request.TenantName, request.TenantSlug);

            var passwordHash = _passwordHasher.Hash(request.Password);

            //create User
            var user = User.Create(
                tenant.Id,
                request.Email,
                request.DisplayName
                );

            user.SetPasswordHash(passwordHash);

            //save to Db
            await _tenantRepository.AddAsync(tenant,cancellationToken);
            await _tenantRepository.SaveChangesAsync(cancellationToken);
            await _userRepository.AddAsync(user,cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            //Token
            var accessToken = _jwtService.GenerateToken(user.Id,
                                                        tenant.Id,
                                                        user.Email);

            //  Generate Refresh Token
            var (rawToken, hash) = _refreshTokenService.Generate();

            var refreshTokenEntity = RefreshToken.Create(
                tenant.Id,
                user.Id,
                hash,
                DateTime.UtcNow.AddDays(7)
            );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity,cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = rawToken
            };

        }

    }
}
