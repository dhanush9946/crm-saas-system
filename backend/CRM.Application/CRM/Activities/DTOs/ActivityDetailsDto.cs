using CRM.Domain.CRM.Enums;

namespace CRM.Application.CRM.Activities.DTOs;

public sealed class ActivityDetailsDto
{
    public Guid Id { get; init; }

    public ActivityType Type { get; init; }

    public string Subject { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public DateTime? OccurredAtUtc { get; init; }

    public DateTime? DueAtUtc { get; init; }

    public DateTime? CompletedAtUtc { get; init; }

    public RelatedEntityType RelatedEntityType { get; init; }

    public Guid RelatedEntityId { get; init; }

    public Guid CreatedByUserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public byte[]? RowVersion { get; init; }
}