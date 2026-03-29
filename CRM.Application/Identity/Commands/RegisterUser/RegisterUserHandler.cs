using CRM.Application.Identity.DTOs.Auth;
using CRM.Domain.Identity.Entities;
using CRM.Application.Common.Exceptions;
using FluentValidation;
using CRM.Application.Identity.Interfaces;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserHandler:IRegisterUserHandler
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

        public async Task<AuthResponseDto> HandleAsync(RegisterUserCommand command)
        {
            var validator = new RegisterUserValidator();
            var result = validator.Validate(command);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            //checks tenant already existed
            var existingTenant = await _tenantRepository
            .GetBySlugAsync(command.TenantSlug);

            if (existingTenant != null)
                throw new ConflictException("Tenant already exists");

            //creating Tenant
            var tenant = Tenant.Create(command.TenantName, command.TenantSlug);

            var passwordHash = _passwordHasher.Hash(command.Password);

            //create User
            var user = User.Create(
                tenant.Id,
                command.Email,
                command.DisplayName
                );

            user.SetPasswordHash(passwordHash);

            //save to Db
            await _tenantRepository.AddAsync(tenant);
            await _userRepository.AddAsync(user);

            //Token
            var accessToken = _jwtService.GenerateToken(user.Id,
                                                        tenant.Id,
                                                        user.Email);

            //  Generate Refresh Token
            var (rawToken, hash) = _refreshTokenService.Generate();

            var refreshTokenEntity = CRM.Domain.Identity.Entities.RefreshToken.Create(
                                                        tenant.Id,
                                                        user.Id,
                                                        hash,
                                                        DateTime.UtcNow.AddDays(7)
                                                    );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

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
