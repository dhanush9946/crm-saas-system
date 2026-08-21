using CRM.Application.Identity.Interfaces;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public sealed class DatabaseAccessTokenStateValidator
    : IAccessTokenStateValidator
{
    private readonly AppDbContext _dbContext;

    public DatabaseAccessTokenStateValidator(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsValidAsync(
        Guid userId,
        Guid tenantId,
        int tokenVersion,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);

        if (user is null)
            return false;

        return user.TenantId == tenantId
            && !user.IsDisabled()
            && user.TokenVersion == tokenVersion;
    }
}