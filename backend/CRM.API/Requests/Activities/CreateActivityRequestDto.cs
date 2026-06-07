using CRM.Domain.CRM.Enums;

namespace CRM.API.Requests.Activities;

public sealed class CreateActivityRequestDto
{
    public ActivityType Type { get; init; }

    public string Subject { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public DateTime? OccurredAtUtc { get; init; }

    public DateTime? DueAtUtc { get; init; }

    public RelatedEntityType RelatedEntityType { get; init; }

    public Guid RelatedEntityId { get; init; }
}