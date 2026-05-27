using CRM.Application.Identity.Commands.CompleteGoogleOnboarding;
using FluentValidation;

namespace CRM.Application.Identity.Validators
{
    public class CompleteGoogleOnboardingCommandValidator
        : AbstractValidator<CompleteGoogleOnboardingCommand>
    {
        public CompleteGoogleOnboardingCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty()
                .MaximumLength(5000);

            RuleFor(x => x.TenantName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(200);

            RuleFor(x => x.TenantSlug)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage(
                    "Slug must be lowercase, alphanumeric, and hyphenated without leading or trailing hyphens");
        }
    }
}
