using CRM.Domain.CRM.Enums;

namespace CRM.API.Contracts.Leads;

public sealed class ConvertLeadToDealRequest
{
    public string Title { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public DealStage Stage { get; init; }

    public DateOnly? ExpectedCloseDate { get; init; }

    public Guid? OwnerUserId { get; init; }
}