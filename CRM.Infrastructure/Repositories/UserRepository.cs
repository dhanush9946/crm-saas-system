

using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(Guid tenantId, string email,CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                    cancellationToken);
        }

        public async Task AddAsync(User user,CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user,cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
