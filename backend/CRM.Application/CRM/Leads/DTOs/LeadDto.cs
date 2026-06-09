namespace CRM.Application.CRM.Leads.DTOs;

public sealed class LeadDto
{
    public Guid Id { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Company { get; init; }

    public string Source { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public decimal? Score { get; init; }

    public Guid? OwnerUserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}