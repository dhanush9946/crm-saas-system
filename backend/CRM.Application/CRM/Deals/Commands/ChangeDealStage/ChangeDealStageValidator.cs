using FluentValidation;

namespace CRM.Application.CRM.Deals.Commands.ChangeDealStage;

public sealed class ChangeDealStageValidator
    : AbstractValidator<ChangeDealStageCommand>
{
    public ChangeDealStageValidator()
    {
        RuleFor(x => x.DealId)
            .NotEmpty();

        RuleFor(x => x.Stage)
            .IsInEnum();
    }
}
