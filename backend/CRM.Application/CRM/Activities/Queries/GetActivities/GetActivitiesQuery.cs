using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.DTOs;
using CRM.Domain.CRM.Enums;
using MediatR;

public sealed class GetActivitiesQuery
    : IRequest<PagedResult<ActivityDto>>
{
    public string? Search { get; init; }

    public RelatedEntityType? RelatedEntityType { get; init; }

    public Guid? RelatedEntityId { get; init; }

    public ActivityType? ActivityType { get; init; }

    public bool? IsCompleted { get; init; }

    public DateTime? DueFrom { get; init; }

    public DateTime? DueTo { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}