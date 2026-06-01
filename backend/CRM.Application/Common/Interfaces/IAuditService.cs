namespace CRM.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(
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
        CancellationToken cancellationToken = default);
}
