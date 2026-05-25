using CRM.Application.Common.Interfaces.Persistence;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Persistence.Repositories.Identity;

public sealed class PasswordResetTokenRepository
    : IPasswordResetTokenRepository
{
    private readonly AppDbContext _context;

    public PasswordResetTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        PasswordResetToken token,
        CancellationToken cancellationToken = default)
    {
        await _context.PasswordResetTokens
            .AddAsync(token, cancellationToken);
    }

    public async Task<PasswordResetToken?> GetUsableByHashAsync(
        Guid tenantId,
        byte[] tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TokenHash == tokenHash &&
                x.UsedAtUtc == null &&
                x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<List<PasswordResetToken>> GetActiveByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetTokens
            .Where(x =>
                x.TenantId == tenantId &&
                x.UserId == userId &&
                x.UsedAtUtc == null &&
                x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeActiveByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeTokens = await GetActiveByUserAsync(tenantId, userId, cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke();
        }
    }
}
