using MediatR;

namespace CRM.Application.CRM.Deals.Commands.DeleteDeal;

public sealed class DeleteDealCommand : IRequest
{
    public Guid DealId { get; set; }
}
