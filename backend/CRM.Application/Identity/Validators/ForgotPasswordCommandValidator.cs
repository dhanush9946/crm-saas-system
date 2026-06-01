using CRM.Application.Identity.Commands.ForgotPassword;
using FluentValidation;

namespace CRM.Application.Identity.Validators;

public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.TenantSlug)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
