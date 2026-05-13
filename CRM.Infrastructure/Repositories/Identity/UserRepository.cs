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
    }
}
