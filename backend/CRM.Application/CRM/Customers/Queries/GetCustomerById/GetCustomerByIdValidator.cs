using FluentValidation;

public sealed class GetCustomerByIdValidator
    : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}