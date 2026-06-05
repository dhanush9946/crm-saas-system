namespace CRM.Application.CRM.Deals.DTOs;

public sealed class GetDealsResponseDto
{
    public IReadOnlyList<DealListItemDto> Items { get; init; }
        = [];

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}
