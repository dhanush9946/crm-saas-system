using MediatR;

namespace CRM.Application.CRM.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommand : IRequest
{
    public Guid CustomerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Website { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}