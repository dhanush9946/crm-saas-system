using FluentValidation;

namespace CRM.Application.CRM.Activities.Queries.GetActivityById;

public sealed class GetActivityByIdValidator
    : AbstractValidator<GetActivityByIdQuery>
{
    public GetActivityByIdValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();
    }
}