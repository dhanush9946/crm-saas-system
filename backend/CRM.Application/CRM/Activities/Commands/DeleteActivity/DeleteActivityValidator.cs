using FluentValidation;

namespace CRM.Application.CRM.Activities.Commands.DeleteActivity;

public sealed class DeleteActivityValidator
    : AbstractValidator<DeleteActivityCommand>
{
    public DeleteActivityValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();

        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}