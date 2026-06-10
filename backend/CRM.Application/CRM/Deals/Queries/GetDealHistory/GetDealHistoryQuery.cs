using CRM.Application.Common.Models;
using CRM.Application.CRM.Deals.DTOs;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDealHistory;

public sealed class GetDealHistoryQuery
    : IRequest<PagedResult<DealHistoryDto>>
{
    public Guid DealId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
