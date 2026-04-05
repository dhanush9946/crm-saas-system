

using CRM.Application.Identity.Commands.RefreshTokenFolder;
using FluentValidation;

namespace CRM.Application.Identity.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
        }
    }
}
