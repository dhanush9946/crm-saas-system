

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(byte[] hash,CancellationToken cancellationToken);

        Task AddAsync(RefreshToken token,CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
