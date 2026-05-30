using CRM.Application.CRM.Customers.Queries.GetCustomers;
using FluentValidation;

public sealed class GetCustomersQueryValidator
    : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(100);

        RuleFor(x => x.SortDirection)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                x.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be asc or desc.");

        RuleFor(x => x.SortBy)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                x.Equals("name", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("createdAtUtc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid sort column.");
    }
}