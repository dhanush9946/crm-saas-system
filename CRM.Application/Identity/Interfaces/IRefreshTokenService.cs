using CRM.Application.Identity.DTOs.Auth;
using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IRefreshTokenService
    {
        (string rawToken, byte[] hash) Generate();

        byte[] Hash(string rawToken);

        Task<RefreshToken?> GetByTokenAsync(string rawToken,CancellationToken cancellationToken);

        Task<RefreshTokenResult> CreateAsync(
            Guid tenantId,
            Guid userId,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken
        );

        Task<RefreshTokenResult> RotateAsync(
            RefreshToken existingToken,
            Guid tenantId,
            Guid userId,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken
        );

        Task RevokeFamilyAsync(
            Guid tokenFamilyId,
            CancellationToken cancellationToken
        );
    }
}
