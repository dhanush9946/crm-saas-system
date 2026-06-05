using CRM.Domain.CRM.Entities;
using FluentValidation;

namespace CRM.Application.CRM.Deals.Commands.UpdateDeal;

public sealed class UpdateDealValidator
    : AbstractValidator<UpdateDealCommand>
{
    public UpdateDealValidator()
    {
        RuleFor(x => x.DealId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Deal.MaxTitleLength);

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}
