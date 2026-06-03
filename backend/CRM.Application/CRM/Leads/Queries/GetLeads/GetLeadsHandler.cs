using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Queries.GetLeads;

public sealed class GetLeadsHandler
    : IRequestHandler<GetLeadsQuery, PagedResult<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;

    public GetLeadsHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<LeadDto>> Handle(
        GetLeadsQuery request,
        CancellationToken cancellationToken)
    {
        var (leads, totalCount) =
            await _leadRepository.GetPagedAsync(
                tenantId: _currentUser.TenantId,
                search: request.Search,
                status: request.Status,
                source: request.Source,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

        var items = leads
            .Select(lead => new LeadDto
            {
                Id = lead.Id,
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                Source = lead.Source.ToString(),
                Status = lead.Status.ToString(),
                Score = lead.Score,
                OwnerUserId = lead.OwnerUserId,
                CreatedAtUtc = lead.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<LeadDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
