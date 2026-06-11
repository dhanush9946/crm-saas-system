using FluentValidation;

namespace CRM.Application.CRM.Leads.Commands.AssignLead;

public sealed class AssignLeadCommandValidator
    : AbstractValidator<AssignLeadCommand>
{
    public AssignLeadCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty();
    }
}