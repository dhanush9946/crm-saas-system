using CRM.Application.Common.Models;
using CRM.Application.CRM.Leads.DTOs;
using MediatR;

namespace CRM.Application.CRM.Leads.Queries.GetLeadHistory;

public sealed class GetLeadHistoryQuery
    : IRequest<PagedResult<LeadHistoryDto>>
{
    public Guid LeadId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
