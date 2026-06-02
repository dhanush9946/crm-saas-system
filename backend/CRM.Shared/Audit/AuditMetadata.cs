namespace CRM.Shared.Audit;

public sealed class AuditMetadata
{
    public string? Action { get; set; }

    public Dictionary<string, object?>? NewValues { get; set; }

    public List<PropertyChange>? Changes { get; set; }
}