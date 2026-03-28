using FluentValidation;


namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.TenantName).NotEmpty();
            RuleFor(x => x.TenantSlug).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).MinimumLength(8);
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}
