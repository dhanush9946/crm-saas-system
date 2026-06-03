namespace CRM.Application.CRM.Leads.DTOs;

public sealed class LeadPropertyChangeDto
{
    public string PropertyName { get; init; } = default!;

    public string? OldValue { get; init; }

    public string? NewValue { get; init; }
}
