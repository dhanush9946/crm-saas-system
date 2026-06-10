using FluentValidation;

public sealed class GetActivitiesValidator
    : AbstractValidator<GetActivitiesQuery>
{
    public GetActivitiesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.ActivityType)
            .IsInEnum()
            .When(x => x.ActivityType.HasValue);

        RuleFor(x => x.RelatedEntityType)
            .IsInEnum()
            .When(x => x.RelatedEntityType.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.DueFrom.HasValue ||
                !x.DueTo.HasValue ||
                x.DueFrom <= x.DueTo)
            .WithMessage(
                "DueFrom must be less than or equal to DueTo.");
    }
}