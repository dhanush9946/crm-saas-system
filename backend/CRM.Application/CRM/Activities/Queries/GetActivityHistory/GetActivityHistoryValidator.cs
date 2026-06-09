using FluentValidation;

namespace CRM.Application.CRM.Activities.Queries.GetActivityHistory;

public sealed class GetActivityHistoryValidator
    : AbstractValidator<GetActivityHistoryQuery>
{
    public GetActivityHistoryValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}