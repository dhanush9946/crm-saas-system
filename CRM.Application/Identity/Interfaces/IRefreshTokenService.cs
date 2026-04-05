using CRM.Application.Identity.DTOs.Auth;
using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IRefreshTokenService
    {
        (string rawToken, byte[] hash) Generate();

        byte[] Hash(string rawToken);

        Task<RefreshToken?> GetByTokenAsync(string rawToken,CancellationToken cancellationToken);

        Task<(string accessToken, string refreshToken)> CreateAsync(
            Guid tenantId,
            Guid userId,
            string email,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken
        );

        Task<AuthResponseDto> RotateAsync(
            RefreshToken existingToken,
            Guid tenantId,
            Guid userId,
            string email,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken
        );
    }
}
