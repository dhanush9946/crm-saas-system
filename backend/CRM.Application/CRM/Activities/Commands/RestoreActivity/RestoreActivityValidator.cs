using FluentValidation;

namespace CRM.Application.CRM.Activities.Commands.RestoreActivity;

public sealed class RestoreActivityValidator
    : AbstractValidator<RestoreActivityCommand>
{
    public RestoreActivityValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();
    }
}