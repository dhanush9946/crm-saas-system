

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.RefreshTokenFolder
{
    public class RefreshTokenHandler:IRequestHandler<RefreshTokenCommand,AuthResponseDto>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            IRefreshTokenService refreshTokenService,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            ILogger<RefreshTokenHandler> logger)
        {
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request,CancellationToken cancellationToken)
        {
            

            // 2. Get token from DB
            var existingToken = await _refreshTokenService
                .GetByTokenAsync(request.RefreshToken,cancellationToken);

            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            // 3. Validate token state
            if (!existingToken.IsActive())
            {
                if (existingToken.IsRevoked() && existingToken.ReplacedByTokenId.HasValue)
                {
                    await _refreshTokenService.RevokeFamilyAsync(
                        existingToken.TokenFamilyId,
                        cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning(
                        "Refresh token reuse detected. TokenId: {TokenId}, TokenFamilyId: {TokenFamilyId}, UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                        existingToken.Id,
                        existingToken.TokenFamilyId,
                        existingToken.UserId,
                        existingToken.TenantId,
                        request.IpAddress,
                        request.DeviceId);

                    throw new UnauthorizedException("Refresh token reuse detected");
                }

                throw new UnauthorizedException("Refresh token expired or revoked");
            }

            // 4. Get user (for email)
            var user = await _userRepository.GetByIdAsync(existingToken.UserId,cancellationToken);

            if (user == null)
                throw new UnauthorizedException("User not found");

            // 5. Fetch roles
            var roles = await _userRoleRepository.GetUserRolesAsync(user.TenantId, user.Id, cancellationToken);

            // 6. Generate new access token
            var accessToken = _jwtService.GenerateToken(user.Id, user.TenantId, user.Email, roles);

            // 7. Rotate token
            var newRawToken = await _refreshTokenService.RotateAsync(
                existingToken,
                user.TenantId,
                user.Id,
                request.DeviceId,
                request.UserAgent,
                request.IpAddress,
                cancellationToken
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = newRawToken
            };
        }

    }
}
