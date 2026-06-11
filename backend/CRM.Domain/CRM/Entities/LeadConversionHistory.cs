using CRM.Domain.Common;
using CRM.Domain.CRM.Enums;

namespace CRM.Domain.CRM.Entities;

public sealed class LeadConversionHistory : BaseEntity
{
    public Guid TenantId { get; private set; }

    public Guid LeadId { get; private set; }

    public LeadConversionType ConversionType { get; private set; }

    public Guid RelatedEntityId { get; private set; }

    public Guid ConvertedByUserId { get; private set; }

    public DateTime ConvertedAtUtc { get; private set; }

    private LeadConversionHistory()
    {
    }

    private LeadConversionHistory(
        Guid tenantId,
        Guid leadId,
        LeadConversionType conversionType,
        Guid relatedEntityId,
        Guid convertedByUserId,
        DateTime convertedAtUtc)
    {
        TenantId = tenantId;
        LeadId = leadId;
        ConversionType = conversionType;
        RelatedEntityId = relatedEntityId;
        ConvertedByUserId = convertedByUserId;
        ConvertedAtUtc = convertedAtUtc;
    }

    public static LeadConversionHistory Create(
    Guid tenantId,
    Guid leadId,
    LeadConversionType conversionType,
    Guid relatedEntityId,
    Guid convertedByUserId,
    DateTime convertedAtUtc)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.");

        if (leadId == Guid.Empty)
            throw new ArgumentException("LeadId is required.");

        if (relatedEntityId == Guid.Empty)
            throw new ArgumentException("RelatedEntityId is required.");

        if (convertedByUserId == Guid.Empty)
            throw new ArgumentException("ConvertedByUserId is required.");

        return new LeadConversionHistory(
            tenantId,
            leadId,
            conversionType,
            relatedEntityId,
            convertedByUserId,
            convertedAtUtc);
    }
}