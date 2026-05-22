using CRM.Application.Identity.Commands.ResetPassword;
using FluentValidation;

namespace CRM.Application.Identity.Validators;

public sealed class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.TenantSlug)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}
