using MediatR;

namespace CRM.Application.CRM.Activities.Commands.CompleteActivity;

public sealed class CompleteActivityCommand : IRequest
{
    public Guid ActivityId { get; init; }

    public string RowVersion { get; init; } = string.Empty;
}