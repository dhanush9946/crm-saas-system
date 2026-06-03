using CRM.Application.Common.Models;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Domain.CRM.Enums;
using CRM.Domain.Leads.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Queries.GetLeads;

public sealed class GetLeadsQuery
    : IRequest<PagedResult<LeadDto>>
{
    public string? Search { get; init; }

    public LeadStatus? Status { get; init; }

    public LeadSource? Source { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
