

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByHashAsync(byte[] hash,CancellationToken cancellationToken);
    }
}
