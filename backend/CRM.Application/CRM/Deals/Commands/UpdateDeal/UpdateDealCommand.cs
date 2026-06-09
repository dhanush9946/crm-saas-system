using MediatR;

namespace CRM.Application.CRM.Deals.Commands.UpdateDeal;

public sealed class UpdateDealCommand : IRequest
{
    public Guid DealId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public DateOnly? ExpectedCloseDate { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}
