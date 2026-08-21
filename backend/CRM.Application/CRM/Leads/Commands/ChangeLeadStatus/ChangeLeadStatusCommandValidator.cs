using FluentValidation;
using CRM.Domain.CRM.Enums;

namespace CRM.Application.CRM.Leads.Commands.ChangeLeadStatus;

public sealed class ChangeLeadStatusCommandValidator
    : AbstractValidator<ChangeLeadStatusCommand>
{
    public ChangeLeadStatusCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();

        // Prevent manual conversion.
        RuleFor(x => x.Status)
            .NotEqual(LeadStatus.Converted)
            .WithMessage(
                "Lead cannot be manually marked as Converted. Use the conversion endpoint.");
    }
}