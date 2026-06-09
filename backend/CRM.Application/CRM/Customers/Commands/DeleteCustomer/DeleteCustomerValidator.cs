using FluentValidation;

namespace CRM.Application.CRM.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerValidator
    : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}