using CRM.Application.CRM.Activities.DTOs;
using MediatR;

namespace CRM.Application.CRM.Activities.Queries.GetActivityById;

public sealed class GetActivityByIdQuery
    : IRequest<ActivityDetailsDto>
{
    public Guid ActivityId { get; init; }
}