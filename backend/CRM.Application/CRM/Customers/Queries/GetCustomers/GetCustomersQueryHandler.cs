using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Customers.DTOs;
using CRM.Application.CRM.Customers.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler
    : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        ICurrentUser currentUser)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<CustomerDto>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var (customers, totalCount) =
            await _customerRepository.GetPagedAsync(
                tenantId: _currentUser.TenantId,
                search: request.Search,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

        var items = customers
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Industry = customer.Industry,
                Website = customer.Website,
                Status = customer.Status.ToString(),
                OwnerUserId = customer.OwnerUserId,
                CreatedAtUtc = customer.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<CustomerDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}