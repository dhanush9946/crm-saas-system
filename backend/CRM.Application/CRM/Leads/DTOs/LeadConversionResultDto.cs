using CRM.Domain.CRM.Enums;

public sealed class LeadConversionResultDto
{
    public Guid LeadId { get; init; }

    public Guid? CustomerId { get; init; }

    public Guid? DealId { get; init; }

    public DateTime ConvertedAtUtc { get; init; }

    public LeadStatus LeadStatus { get; init; }
}