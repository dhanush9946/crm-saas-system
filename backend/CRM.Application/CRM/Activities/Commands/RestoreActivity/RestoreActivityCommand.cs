using MediatR;

namespace CRM.Application.CRM.Activities.Commands.RestoreActivity;

public sealed class RestoreActivityCommand : IRequest
{
    public Guid ActivityId { get; init; }
}