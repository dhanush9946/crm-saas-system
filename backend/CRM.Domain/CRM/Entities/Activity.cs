using CRM.Domain.Common;
using CRM.Domain.Common.Interfaces;
using CRM.Domain.CRM.Enums;

namespace CRM.Domain.CRM.Entities;

public sealed class Activity : BaseEntity, IAuditable
{
    public const int MaxSubjectLength = 200;
    public const int MaxNotesLength = 5000;

    public Guid TenantId { get; private set; }

    public ActivityType Type { get; private set; }

    public string Subject { get; private set; } = default!;

    public string? Notes { get; private set; }

    public DateTime? OccurredAtUtc { get; private set; }

    public DateTime? DueAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public RelatedEntityType RelatedEntityType { get; private set; }

    public Guid RelatedEntityId { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool IsCompleted => CompletedAtUtc.HasValue;

    private Activity()
    {
    }

    private Activity(
        Guid tenantId,
        ActivityType type,
        string subject,
        string? notes,
        DateTime? occurredAtUtc,
        DateTime? dueAtUtc,
        RelatedEntityType relatedEntityType,
        Guid relatedEntityId,
        Guid createdByUserId)
    {
        TenantId = tenantId;
        Type = type;
        Subject = subject;
        Notes = notes;
        OccurredAtUtc = occurredAtUtc;
        DueAtUtc = dueAtUtc;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        CreatedByUserId = createdByUserId;

        IsDeleted = false;
    }

    public static Activity Create(
        Guid tenantId,
        ActivityType type,
        string subject,
        string? notes,
        DateTime? occurredAtUtc,
        DateTime? dueAtUtc,
        RelatedEntityType relatedEntityType,
        Guid relatedEntityId,
        Guid createdByUserId)
    {
        ValidateTenant(tenantId);
        ValidateSubject(subject);
        ValidateNotes(notes);
        ValidateRelatedEntity(relatedEntityId);
        ValidateCreatedBy(createdByUserId);
        ValidateActivityRules(
            type,
            occurredAtUtc,
            dueAtUtc);

        return new Activity(
            tenantId,
            type,
            subject.Trim(),
            notes?.Trim(),
            occurredAtUtc,
            dueAtUtc,
            relatedEntityType,
            relatedEntityId,
            createdByUserId);
    }

    public void Update(
        ActivityType type,
        string subject,
        string? notes,
        DateTime? occurredAtUtc,
        DateTime? dueAtUtc)
    {
        EnsureNotDeleted();

        ValidateSubject(subject);
        ValidateNotes(notes);
        ValidateActivityRules(
            type,
            occurredAtUtc,
            dueAtUtc);

        Type = type;
        Subject = subject.Trim();
        Notes = notes?.Trim();
        OccurredAtUtc = occurredAtUtc;
        DueAtUtc = dueAtUtc;

        SetUpdated();
    }

    public void Complete(DateTime completedAtUtc)
    {
        EnsureNotDeleted();

        if (CompletedAtUtc.HasValue)
        {
            throw new InvalidOperationException(
                "Activity is already completed.");
        }

        if (completedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Completed date cannot be earlier than created date.");
        }

        CompletedAtUtc = completedAtUtc;

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
        {
            return;
        }

        IsDeleted = false;

        SetUpdated();
    }

    private static void ValidateTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException(
                "TenantId is required.");
        }
    }

    private static void ValidateRelatedEntity(
        Guid relatedEntityId)
    {
        if (relatedEntityId == Guid.Empty)
        {
            throw new ArgumentException(
                "RelatedEntityId is required.");
        }
    }

    private static void ValidateCreatedBy(
        Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException(
                "CreatedByUserId is required.");
        }
    }

    private static void ValidateSubject(
        string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException(
                "Activity subject is required.");
        }

        if (subject.Length > MaxSubjectLength)
        {
            throw new ArgumentException(
                $"Activity subject cannot exceed {MaxSubjectLength} characters.");
        }
    }

    private static void ValidateNotes(
        string? notes)
    {
        if (notes is not null &&
            notes.Length > MaxNotesLength)
        {
            throw new ArgumentException(
                $"Activity notes cannot exceed {MaxNotesLength} characters.");
        }
    }

    private static void ValidateActivityRules(
        ActivityType type,
        DateTime? occurredAtUtc,
        DateTime? dueAtUtc)
    {
        switch (type)
        {
            case ActivityType.Call:
            case ActivityType.Meeting:

                if (!occurredAtUtc.HasValue)
                {
                    throw new ArgumentException(
                        "OccurredAtUtc is required for calls and meetings.");
                }

                break;

            case ActivityType.Task:

                if (!dueAtUtc.HasValue)
                {
                    throw new ArgumentException(
                        "DueAtUtc is required for task activities.");
                }

                break;

            case ActivityType.Note:
                break;

            default:
                throw new ArgumentException(
                    "Invalid activity type.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Deleted activity cannot be modified.");
        }
    }

    public bool IsOverdue()
    {
        return Type == ActivityType.Task
               && !CompletedAtUtc.HasValue
               && DueAtUtc.HasValue
               && DueAtUtc.Value < DateTime.UtcNow;
    }
}