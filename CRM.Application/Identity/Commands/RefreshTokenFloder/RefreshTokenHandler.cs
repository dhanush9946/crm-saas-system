

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.Commands.RefreshTokenFloder;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;

namespace CRM.Application.Identity.Commands.RefreshToken
{
    public class RefreshTokenHandler:IRefreshTokenHandler
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenHandler(
            IRefreshTokenService refreshTokenService,
            IUserRepository userRepository)
        {
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> HandleAsync(RefreshTokenCommand command)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(command.RefreshToken))
                throw new BadRequestException("Refresh token is required");

            // 2. Get token from DB
            var existingToken = await _refreshTokenService
                .GetByTokenAsync(command.RefreshToken);

            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            // 3. Validate token state
            if (!existingToken.IsActive())
                throw new UnauthorizedException("Refresh token expired or revoked");

            // 4. Get user (for email)
            var user = await _userRepository.GetByIdAsync(existingToken.UserId);

            if (user == null)
                throw new UnauthorizedException("User not found");

            // 5. Rotate token
            var result = await _refreshTokenService.RotateAsync(
                existingToken,
                user.TenantId,
                user.Id,
                user.Email, 
                command.DeviceId,
                command.UserAgent,
                command.IpAddress
            );

            return result;
        }

    }
}
