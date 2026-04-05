

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using MediatR;

namespace CRM.Application.Identity.Commands.RefreshTokenFolder
{
    public class RefreshTokenHandler:IRequestHandler<RefreshTokenCommand,AuthResponseDto>
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

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request,CancellationToken cancellationToken)
        {
            

            // 2. Get token from DB
            var existingToken = await _refreshTokenService
                .GetByTokenAsync(request.RefreshToken,cancellationToken);

            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            // 3. Validate token state
            if (!existingToken.IsActive())
                throw new UnauthorizedException("Refresh token expired or revoked");

            // 4. Get user (for email)
            var user = await _userRepository.GetByIdAsync(existingToken.UserId,cancellationToken);

            if (user == null)
                throw new UnauthorizedException("User not found");

            // 5. Rotate token
            var result = await _refreshTokenService.RotateAsync(
                existingToken,
                user.TenantId,
                user.Id,
                user.Email, 
                request.DeviceId,
                request.UserAgent,
                request.IpAddress,
                cancellationToken
            );

            return result;
        }

    }
}
