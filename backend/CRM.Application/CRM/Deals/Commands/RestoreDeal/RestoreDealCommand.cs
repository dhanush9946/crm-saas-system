using MediatR;

namespace CRM.Application.CRM.Deals.Commands.RestoreDeal;

public sealed class RestoreDealCommand : IRequest
{
    public Guid DealId { get; set; }
}
