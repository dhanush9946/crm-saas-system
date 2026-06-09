using FluentValidation;

namespace CRM.Application.CRM.Deals.Queries.GetDeals;

public sealed class GetDealsQueryValidator
    : AbstractValidator<GetDealsQuery>
{
    public GetDealsQueryValidator()
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
                x.Equals("title", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("value", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("probability", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("expectedCloseDate", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("createdAtUtc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid sort column.");

        RuleFor(x => x)
            .Must(x =>
                !x.ExpectedCloseFrom.HasValue ||
                !x.ExpectedCloseTo.HasValue ||
                x.ExpectedCloseFrom.Value <= x.ExpectedCloseTo.Value)
            .WithMessage("ExpectedCloseFrom must be before ExpectedCloseTo.");
    }
}
