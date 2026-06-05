namespace CRM.Application.CRM.Deals.DTOs;

public sealed class DealHistoryDto
{
    public string Action { get; init; } = string.Empty;

    public Guid? UserId { get; init; }

    public Guid? ChangedByUserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime Timestamp { get; init; }

    public string? EntityType { get; init; }

    public string? EntityId { get; init; }

    public bool Succeeded { get; init; }

    public string? FailureReason { get; init; }

    public string? ChangesJson { get; init; }

    public IReadOnlyList<DealPropertyChangeDto> Changes { get; init; }
        = [];
}
