using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories.Identity;

public sealed class ExternalLoginRepository
    : Repository<ExternalLogin>, IExternalLoginRepository
{
    public ExternalLoginRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ExternalLogin?> GetByProviderAsync(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken)
    {
        var normalizedProvider = provider.Trim().ToUpperInvariant();
        var normalizedProviderUserId = providerUserId.Trim();

        return await _context.ExternalLogins
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x =>
                    x.Provider == normalizedProvider &&
                    x.ProviderUserId == normalizedProviderUserId,
                cancellationToken);
    }
}
