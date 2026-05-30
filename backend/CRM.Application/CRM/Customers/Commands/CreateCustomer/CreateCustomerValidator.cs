using FluentValidation;

namespace CRM.Application.CRM.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Industry)
            .MaximumLength(100);

        RuleFor(x => x.Website)
            .MaximumLength(300);
    }
}