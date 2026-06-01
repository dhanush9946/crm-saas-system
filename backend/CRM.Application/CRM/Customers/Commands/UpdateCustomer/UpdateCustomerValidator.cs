using FluentValidation;

namespace CRM.Application.CRM.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerValidator
    : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Industry)
            .MaximumLength(100);

        RuleFor(x => x.Website)
            .MaximumLength(300);

        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}