

using CRM.Application.Identity.Commands.Login;
using FluentValidation;

namespace CRM.Application.Identity.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.TenantSlug)
                .NotEmpty()
                .MinimumLength(3);
        }
    }
}
