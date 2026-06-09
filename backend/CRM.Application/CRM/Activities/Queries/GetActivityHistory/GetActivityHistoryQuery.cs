using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.DTOs;
using MediatR;

namespace CRM.Application.CRM.Activities.Queries.GetActivityHistory;

public sealed class GetActivityHistoryQuery
    : IRequest<PagedResult<ActivityHistoryDto>>
{
    public Guid ActivityId { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}