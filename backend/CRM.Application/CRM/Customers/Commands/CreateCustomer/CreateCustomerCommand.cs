using MediatR;

namespace CRM.Application.CRM.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string? Industry,
    string? Website,
    Guid? OwnerUserId
) : IRequest<Guid>;