using CRM.Domain.Identity.Entities;

namespace CRM.Application.Common.Interfaces.Persistence;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(
        PasswordResetToken token,
        CancellationToken cancellationToken = default);

    Task<PasswordResetToken?> GetUsableByHashAsync(
        Guid tenantId,
        byte[] tokenHash,
        CancellationToken cancellationToken = default);

    Task<List<PasswordResetToken>> GetActiveByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RevokeActiveByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
