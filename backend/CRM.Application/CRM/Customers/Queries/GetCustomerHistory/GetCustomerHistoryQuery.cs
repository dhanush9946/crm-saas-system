using CRM.Application.Common.Models;
using CRM.Application.Customers.DTOs;
using MediatR;

namespace CRM.Application.Customers.Queries.GetCustomerHistory;

public sealed class GetCustomerHistoryQuery
    : IRequest<PagedResult<CustomerHistoryDto>>
{
    public Guid CustomerId { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}