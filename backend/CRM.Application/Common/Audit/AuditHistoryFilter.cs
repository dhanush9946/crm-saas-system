public sealed record AuditHistoryFilter(
    Guid TenantId,
    string EntityType,
    string EntityId,
    int Page,
    int PageSize);