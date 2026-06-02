using CRM.Application.Common.Models;
using CRM.Domain.Common;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLog>> GetEntityHistoryAsync(
        AuditHistoryFilter filter,
        CancellationToken cancellationToken = default);
}