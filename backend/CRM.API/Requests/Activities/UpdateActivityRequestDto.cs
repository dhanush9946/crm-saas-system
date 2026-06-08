using CRM.Domain.CRM.Enums;

public sealed class UpdateActivityRequestDto
{
    public ActivityType Type { get; init; }

    public string Subject { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public DateTime? OccurredAtUtc { get; init; }

    public DateTime? DueAtUtc { get; init; }

    public string RowVersion { get; init; } = string.Empty;
}