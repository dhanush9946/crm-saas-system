namespace CRM.Shared.Audit;

public sealed class PropertyChange
{
    public string PropertyName { get; set; } = default!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}