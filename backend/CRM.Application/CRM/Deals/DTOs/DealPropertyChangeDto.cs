namespace CRM.Application.CRM.Deals.DTOs;

public sealed class DealPropertyChangeDto
{
    public string PropertyName { get; init; } = string.Empty;

    public string? OldValue { get; init; }

    public string? NewValue { get; init; }
}
