using FluentValidation;

public sealed class RestoreCustomerValidator
    : AbstractValidator<RestoreCustomerCommand>
{
    public RestoreCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}