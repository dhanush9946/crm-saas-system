using FluentValidation;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToCustomer;

public sealed class ConvertLeadToCustomerCommandValidator
    : AbstractValidator<ConvertLeadToCustomerCommand>
{
    public ConvertLeadToCustomerCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty();
    }
}
