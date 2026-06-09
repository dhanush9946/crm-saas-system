using CRM.Domain.Leads.Enums;

namespace CRM.API.Requests.Leads;

public sealed class UpdateLeadRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Company { get; set; }

    public LeadSource Source { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}
