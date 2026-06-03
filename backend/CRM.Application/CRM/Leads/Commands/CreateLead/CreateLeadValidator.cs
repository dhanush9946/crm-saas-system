using CRM.Domain.CRM.Entities;
using FluentValidation;

namespace CRM.Application.CRM.Leads.Commands.CreateLead;

public sealed class CreateLeadValidator
    : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(Lead.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .MaximumLength(Lead.MaxLastNameLength);

        RuleFor(x => x.Email)
            .MaximumLength(Lead.MaxEmailLength)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(Lead.MaxPhoneLength);

        RuleFor(x => x.Company)
            .MaximumLength(Lead.MaxCompanyLength);

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.Email)
                || !string.IsNullOrWhiteSpace(x.Phone)
                || !string.IsNullOrWhiteSpace(x.Company))
            .WithMessage(
                "At least one of Email, Phone, or Company must be provided.");
    }
}