using FluentValidation;

namespace CRM.Application.CRM.Activities.Commands.CompleteActivity;

public sealed class CompleteActivityValidator
    : AbstractValidator<CompleteActivityCommand>
{
    public CompleteActivityValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();

        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}