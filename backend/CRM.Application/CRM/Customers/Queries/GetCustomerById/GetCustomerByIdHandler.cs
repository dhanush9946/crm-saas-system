using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.DTOs;
using CRM.Application.CRM.Customers.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdHandler
    : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;

    public GetCustomerByIdHandler(
        ICustomerRepository customerRepository,
        ICurrentUser currentUser)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
    }

    public async Task<CustomerDetailsDto> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Customer '{request.CustomerId}' was not found.");
        }

        return new CustomerDetailsDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Industry = customer.Industry,
            Website = customer.Website,
            Status = customer.Status.ToString(),
            OwnerUserId = customer.OwnerUserId,
            CreatedAtUtc = customer.CreatedAtUtc,
            UpdatedAtUtc = customer.UpdatedAtUtc,
            RowVersion = Convert.ToBase64String(
                                customer.RowVersion!)
        };
    }
}