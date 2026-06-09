using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using FluentValidation;

namespace CRM.Application.CRM.Activities.Commands.UpdateActivity;

public sealed class UpdateActivityValidator
    : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(Activity.MaxSubjectLength);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.RowVersion)
            .NotEmpty();

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