using MediatR;

namespace CRM.Application.CRM.Leads.Commands.AssignLead;

public sealed class AssignLeadCommand : IRequest
{
    public Guid LeadId { get; set; }

    public Guid? OwnerUserId { get; set; }
}