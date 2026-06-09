namespace CRM.Application.Customers.DTOs;

public sealed class PropertyChangeDto
{
    public string PropertyName { get; init; } = default!;

    public string? OldValue { get; init; }

    public string? NewValue { get; init; }
}