using CRM.Application.Identity.Commands.RegisterUser;
using FluentValidation;


namespace CRM.Application.Identity.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[a-z]").WithMessage("Must contain lowercase")
                .Matches("[0-9]").WithMessage("Must contain number");

            RuleFor(x => x.TenantName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.TenantSlug)
                .NotEmpty()
                .Matches("^[a-z0-9-]+$")
                .WithMessage("Slug must be lowercase and hyphenated");
        }
    }
}
