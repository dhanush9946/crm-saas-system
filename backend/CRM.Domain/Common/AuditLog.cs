namespace CRM.Domain.Common;

public sealed class AuditLog
{
    public Guid Id { get; private set; }

    public Guid? UserId { get; private set; }

    public Guid? TenantId { get; private set; }

    public string Action { get; private set; } = default!;

    public string? EntityType { get; private set; }

    public string? EntityId { get; private set; }

    public bool Succeeded { get; private set; }

    public string? FailureReason { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? DeviceId { get; private set; }

    public string? TraceId { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private AuditLog() { }

    private AuditLog(
        string action,
        Guid? userId,
        Guid? tenantId,
        string? entityType,
        string? entityId,
        bool succeeded,
        string? failureReason,
        string? ipAddress,
        string? userAgent,
        string? deviceId,
        string? traceId,
        string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Audit action is required");

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId cannot be empty");

        Id = Guid.NewGuid();
        UserId = userId;
        TenantId = tenantId;
        Action = action.Trim();
        EntityType = Normalize(entityType);
        EntityId = Normalize(entityId);
        Succeeded = succeeded;
        FailureReason = Normalize(failureReason);
        IpAddress = Normalize(ipAddress);
        UserAgent = Normalize(userAgent);
        DeviceId = Normalize(deviceId);
        TraceId = Normalize(traceId);
        MetadataJson = Normalize(metadataJson);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static AuditLog Create(
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
        string? metadataJson = null)
    {
        return new AuditLog(
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
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
