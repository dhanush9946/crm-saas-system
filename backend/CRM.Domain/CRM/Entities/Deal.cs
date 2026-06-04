using CRM.Domain.Common;
using CRM.Domain.Common.Interfaces;
using CRM.Domain.CRM.Enums;

namespace CRM.Domain.CRM.Entities;

public sealed class Deal : BaseEntity, IAuditable
{
    public const int MaxTitleLength = 200;

    public Guid TenantId { get; private set; }

    public string Title { get; private set; } = default!;

    public Guid CustomerId { get; private set; }

    public Guid? LeadId { get; private set; }

    public decimal Value { get; private set; }

    public decimal Probability { get; private set; }

    public DealStage Stage { get; private set; }

    public DateOnly? ExpectedCloseDate { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public bool IsDeleted { get; private set; }

    private Deal()
    {
    }

    private Deal(
        Guid tenantId,
        string title,
        Guid customerId,
        Guid? leadId,
        decimal value,
        DealStage stage,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId)
    {
        TenantId = tenantId;
        Title = title;
        CustomerId = customerId;
        LeadId = leadId;
        Value = value;
        Stage = stage;
        Probability = GetProbability(stage);
        ExpectedCloseDate = expectedCloseDate;
        OwnerUserId = ownerUserId;

        IsDeleted = false;
    }

    public static Deal Create(
        Guid tenantId,
        string title,
        Guid customerId,
        Guid? leadId,
        decimal value,
        DealStage stage,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId)
    {
        ValidateTenant(tenantId);
        ValidateTitle(title);
        ValidateCustomer(customerId);
        ValidateValue(value);

        return new Deal(
            tenantId,
            title.Trim(),
            customerId,
            leadId,
            value,
            stage,
            expectedCloseDate,
            ownerUserId);
    }

    public void Update(
        string title,
        decimal value,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId)
    {
        EnsureNotDeleted();

        ValidateTitle(title);
        ValidateValue(value);

        Title = title.Trim();
        Value = value;
        ExpectedCloseDate = expectedCloseDate;
        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void AssignOwner(Guid? ownerUserId)
    {
        EnsureNotDeleted();

        if (OwnerUserId == ownerUserId)
            return;

        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void ChangeStage(DealStage stage)
    {
        EnsureNotDeleted();

        if (Stage == stage)
            return;

        if (!IsValidStageTransition(Stage, stage))
        {
            throw new InvalidOperationException(
                $"Cannot move deal from {Stage} to {stage}.");
        }

        Stage = stage;
        Probability = GetProbability(stage);

        SetUpdated();
    }

    public void SoftDelete()
    {
        EnsureNotDeleted();

        IsDeleted = true;

        SetUpdated();
    }

    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;

        SetUpdated();
    }

    private static decimal GetProbability(
        DealStage stage)
    {
        return stage switch
        {
            DealStage.Qualification => 25m,
            DealStage.Proposal => 50m,
            DealStage.Negotiation => 75m,
            DealStage.Won => 100m,
            DealStage.Lost => 0m,
            _ => 0m
        };
    }

    private static bool IsValidStageTransition(
        DealStage currentStage,
        DealStage newStage)
    {
        return currentStage switch
        {
            DealStage.Qualification =>
                newStage is DealStage.Proposal
                    or DealStage.Lost,

            DealStage.Proposal =>
                newStage is DealStage.Negotiation
                    or DealStage.Lost,

            DealStage.Negotiation =>
                newStage is DealStage.Won
                    or DealStage.Lost,

            DealStage.Won => false,

            DealStage.Lost => false,

            _ => false
        };
    }

    private static void ValidateTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "TenantId is required.");
        }
    }

    private static void ValidateCustomer(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "CustomerId is required.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Deal title is required.");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException(
                $"Deal title cannot exceed {MaxTitleLength} characters.");
        }
    }

    private static void ValidateValue(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException(
                "Deal value cannot be negative.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Deleted deal cannot be modified.");
        }
    }
}