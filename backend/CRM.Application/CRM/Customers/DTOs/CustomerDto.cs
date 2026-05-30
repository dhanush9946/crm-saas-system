namespace CRM.Application.CRM.Customers.DTOs;

public sealed class CustomerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Industry { get; init; }

    public string? Website { get; init; }

    public string Status { get; init; } = string.Empty;

    public Guid? OwnerUserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}