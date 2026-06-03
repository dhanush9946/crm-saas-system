using CRM.Domain.Leads.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommand : IRequest
{
    public Guid LeadId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Company { get; set; }

    public LeadSource Source { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}
