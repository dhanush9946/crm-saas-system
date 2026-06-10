using CRM.Domain.CRM.Enums;

namespace CRM.API.Requests.Leads;

public sealed class ChangeLeadStatusRequest
{
    public LeadStatus Status { get; set; }
}