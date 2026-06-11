using CRM.Domain.CRM.Entities;
using FluentValidation;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToDeal;

public sealed class ConvertLeadToDealValidator
    : AbstractValidator<ConvertLeadToDealCommand>
{
    public ConvertLeadToDealValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Deal.MaxTitleLength);

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Stage)
            .IsInEnum();
    }
}