using MediatR;

namespace CRM.Application.CRM.Activities.Commands.DeleteActivity;

public sealed class DeleteActivityCommand : IRequest
{
    public Guid ActivityId { get; init; }

    public string RowVersion { get; init; } = string.Empty;
}