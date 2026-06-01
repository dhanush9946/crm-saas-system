using CRM.Application.Common.Interfaces;
using CRM.Domain.Common;
using CRM.Infrastructure.Persistence;

namespace CRM.Infrastructure.Services;

public sealed class ConcurrencyService
    : IConcurrencyService
{
    private readonly AppDbContext _context;

    public ConcurrencyService(
        AppDbContext context)
    {
        _context = context;
    }

    public void SetOriginalRowVersion(
        BaseEntity entity,
        byte[] rowVersion)
    {
        _context.Entry(entity)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;
    }
}