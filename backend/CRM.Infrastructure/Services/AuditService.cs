using CRM.Application.Common.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;

namespace CRM.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        string action,
        Guid? userId = null,
        Guid? tenantId = null,
        string? entityType = null,
        string? entityId = null,
        bool succeeded = true,
        string? failureReason = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceId = null,
        string? traceId = null,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = AuditLog.Create(
            action,
            userId,
            tenantId,
            entityType,
            entityId,
            succeeded,
            failureReason,
            ipAddress,
            userAgent,
            deviceId,
            traceId,
            metadataJson);

        await _context.AuditLogs.AddAsync(
            auditLog,
            cancellationToken);
    }
}
