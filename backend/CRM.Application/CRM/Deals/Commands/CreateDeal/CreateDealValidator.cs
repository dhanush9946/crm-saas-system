using CRM.Domain.CRM.Entities;
using FluentValidation;

namespace CRM.Application.CRM.Deals.Commands.CreateDeal;

public sealed class CreateDealValidator
    : AbstractValidator<CreateDealCommand>
{
    public CreateDealValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Deal.MaxTitleLength);

        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0);
    }
}
