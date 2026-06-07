using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using FluentValidation;

namespace CRM.Application.CRM.Activities.Commands.CreateActivity;

public sealed class CreateActivityValidator
    : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityValidator()
    {
        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(Activity.MaxSubjectLength);

        RuleFor(x => x.RelatedEntityId)
            .NotEmpty();

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.RelatedEntityType)
            .IsInEnum();

        RuleFor(x => x)
            .Must(x =>
                x.Type != ActivityType.Task ||
                x.DueAtUtc.HasValue)
            .WithMessage(
                "DueAtUtc is required for task activities.");

        RuleFor(x => x)
            .Must(x =>
                x.Type is not ActivityType.Call
                and not ActivityType.Meeting
                || x.OccurredAtUtc.HasValue)
            .WithMessage(
                "OccurredAtUtc is required for calls and meetings.");
    }
}