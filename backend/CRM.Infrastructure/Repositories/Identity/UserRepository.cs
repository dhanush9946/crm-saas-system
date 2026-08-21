using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories.Identity
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(Guid tenantId, string email,CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim().ToUpperInvariant();
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.EmailNormalized == normalizedEmail,
                    cancellationToken);
        }

        public async Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.EmailNormalized == normalizedEmail, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(
            Guid tenantId,
            Guid userId,
            CancellationToken cancellationToken
            )
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.Id == userId,
                cancellationToken
                );
        }
    }
}
