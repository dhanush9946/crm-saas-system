using MediatR;
using CRM.Domain.Leads.Enums;

namespace CRM.Application.CRM.Leads.Commands.CreateLead;

public sealed class CreateLeadCommand : IRequest<Guid>
{
    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Company { get; init; }

    public LeadSource Source { get; init; }

    public Guid? OwnerUserId { get; init; }
}