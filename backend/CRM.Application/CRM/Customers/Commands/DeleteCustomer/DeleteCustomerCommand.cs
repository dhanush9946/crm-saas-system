using MediatR;

namespace CRM.Application.CRM.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommand : IRequest
{
    public Guid CustomerId { get; set; }
}