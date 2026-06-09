using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Deals.DTOs;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDeals;

public sealed class GetDealsHandler
    : IRequestHandler<GetDealsQuery, PagedResult<DealListItemDto>>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;

    public GetDealsHandler(
        IDealRepository dealRepository,
        ICurrentUser currentUser)
    {
        _dealRepository = dealRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<DealListItemDto>> Handle(
        GetDealsQuery request,
        CancellationToken cancellationToken)
    {
        var (deals, totalCount) =
            await _dealRepository.GetPagedAsync(
                tenantId: _currentUser.TenantId,
                search: request.Search,
                stage: request.Stage,
                ownerUserId: request.OwnerUserId,
                customerId: request.CustomerId,
                expectedCloseFrom: request.ExpectedCloseFrom,
                expectedCloseTo: request.ExpectedCloseTo,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

        var items = deals
            .Select(deal => new DealListItemDto
            {
                Id = deal.Id,
                Title = deal.Title,
                CustomerId = deal.CustomerId,
                LeadId = deal.LeadId,
                Value = deal.Value,
                Probability = deal.Probability,
                Stage = deal.Stage.ToString(),
                ExpectedCloseDate = deal.ExpectedCloseDate,
                OwnerUserId = deal.OwnerUserId,
                CreatedAtUtc = deal.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<DealListItemDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
