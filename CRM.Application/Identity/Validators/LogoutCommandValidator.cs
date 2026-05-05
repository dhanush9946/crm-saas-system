using CRM.Application.Identity.Commands.Logout;
using FluentValidation;

namespace CRM.Application.Identity.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
        }
    }
}
