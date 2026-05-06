using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories.Identity
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByHashAsync(byte[] hash,CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash,cancellationToken);
        }

        public async Task<List<RefreshToken>> GetByFamilyIdAsync(Guid tokenFamilyId, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Where(x => x.TokenFamilyId == tokenFamilyId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<RefreshToken>> GetActiveByUserAsync(
            Guid tenantId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.UserId == userId &&
                    x.RevokedAtUtc == null &&
                    x.ExpiresAtUtc > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }
    }
}
