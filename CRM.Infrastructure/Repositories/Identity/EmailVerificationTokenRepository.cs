using CRM.Application.Common.Interfaces.Persistence;
using CRM.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Persistence.Repositories.Identity;

public sealed class EmailVerificationTokenRepository
    : IEmailVerificationTokenRepository
{
    private readonly AppDbContext _context;

    public EmailVerificationTokenRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        EmailVerificationToken token,
        CancellationToken cancellationToken = default)
    {
        await _context.EmailVerificationTokens
            .AddAsync(token, cancellationToken);
    }

    public async Task<EmailVerificationToken?> GetByTokenHashAsync(
    byte[] tokenHash,
    CancellationToken cancellationToken = default)
    {
        return await _context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task<List<EmailVerificationToken>>
    GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailVerificationTokens
            .Where(x =>
                x.UserId == userId &&
                x.UsedAtUtc == null &&
                x.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}