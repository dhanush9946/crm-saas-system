using CRM.Domain.Identity.Entities;

namespace CRM.Application.Common.Interfaces.Persistence;

public interface IEmailVerificationTokenRepository
{
    Task AddAsync(
        EmailVerificationToken token,
        CancellationToken cancellationToken = default);

    Task<EmailVerificationToken?> GetByTokenHashAsync(
        byte[] tokenHash,
        CancellationToken cancellationToken = default);

    Task<List<EmailVerificationToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

}