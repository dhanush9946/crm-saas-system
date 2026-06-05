namespace CRM.Application.CRM.Deals.DTOs;

public sealed class DealDetailsDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public Guid CustomerId { get; init; }

    public Guid? LeadId { get; init; }

    public decimal Value { get; init; }

    public decimal Probability { get; init; }

    public string Stage { get; init; } = string.Empty;

    public DateOnly? ExpectedCloseDate { get; init; }

    public Guid? OwnerUserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public string RowVersion { get; init; } = string.Empty;
}
