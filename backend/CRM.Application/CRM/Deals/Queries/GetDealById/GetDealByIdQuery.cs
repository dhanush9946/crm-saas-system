using CRM.Application.CRM.Deals.DTOs;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDealById;

public sealed class GetDealByIdQuery
    : IRequest<DealDetailsDto>
{
    public Guid DealId { get; set; }
}
