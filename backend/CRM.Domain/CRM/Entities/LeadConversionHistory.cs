using CRM.Domain.Common;

namespace CRM.Domain.CRM.Entities;

public sealed class LeadConversionHistory : BaseEntity
{
    public Guid TenantId { get; private set; }

    public Guid LeadId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid ConvertedByUserId { get; private set; }

    public DateTime ConvertedAtUtc { get; private set; }

    private LeadConversionHistory()
    {
    }

    private LeadConversionHistory(
        Guid tenantId,
        Guid leadId,
        Guid customerId,
        Guid convertedByUserId,
        DateTime convertedAtUtc)
    {
        TenantId = tenantId;
        LeadId = leadId;
        CustomerId = customerId;
        ConvertedByUserId = convertedByUserId;
        ConvertedAtUtc = convertedAtUtc;
    }

    public static LeadConversionHistory Create(
        Guid tenantId,
        Guid leadId,
        Guid customerId,
        Guid convertedByUserId,
        DateTime convertedAtUtc)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.");

        if (leadId == Guid.Empty)
            throw new ArgumentException("LeadId is required.");

        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.");

        if (convertedByUserId == Guid.Empty)
            throw new ArgumentException("ConvertedByUserId is required.");

        return new LeadConversionHistory(
            tenantId,
            leadId,
            customerId,
            convertedByUserId,
            convertedAtUtc);
    }
}