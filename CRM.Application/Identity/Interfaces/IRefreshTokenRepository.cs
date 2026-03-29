

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(byte[] hash);

        Task AddAsync(RefreshToken token);

        Task SaveChangesAsync();
    }
}
