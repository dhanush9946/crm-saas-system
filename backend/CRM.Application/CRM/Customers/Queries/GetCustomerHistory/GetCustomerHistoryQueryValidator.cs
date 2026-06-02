using FluentValidation;

namespace CRM.Application.Customers.Queries.GetCustomerHistory;

public sealed class GetCustomerHistoryQueryValidator
    : AbstractValidator<GetCustomerHistoryQuery>
{
    public GetCustomerHistoryQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}