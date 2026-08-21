using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ChangeLeadStatus;

public sealed class ChangeLeadStatusCommand : IRequest
{
    public Guid LeadId { get; set; }

    public LeadStatus Status { get; set; }
}