using CRM.Domain.CRM.Enums;

namespace CRM.API.Requests.Deals;

public sealed class CreateDealRequestDto
{
    public string Title { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public Guid? LeadId { get; set; }

    public decimal Value { get; set; }

    public DealStage Stage { get; set; }

    public DateOnly? ExpectedCloseDate { get; set; }

    public Guid? OwnerUserId { get; set; }
}
