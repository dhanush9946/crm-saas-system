using MediatR;

namespace CRM.Application.CRM.Leads.Commands.DeleteLead;

public sealed class DeleteLeadCommand : IRequest
{
    public Guid LeadId { get; set; }
}
