using CRM.Application.CRM.Customers.DTOs;
using MediatR;

public sealed class GetCustomerByIdQuery
    : IRequest<CustomerDetailsDto>
{
    public Guid CustomerId { get; set; }
}