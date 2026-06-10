using CRM.Application.Common.Models;
using CRM.Application.CRM.Deals.DTOs;
using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDeals;

public sealed class GetDealsQuery
    : IRequest<PagedResult<DealListItemDto>>
{
    public string? Search { get; init; }

    public DealStage? Stage { get; init; }

    public Guid? OwnerUserId { get; init; }

    public Guid? CustomerId { get; init; }

    public DateOnly? ExpectedCloseFrom { get; init; }

    public DateOnly? ExpectedCloseTo { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
