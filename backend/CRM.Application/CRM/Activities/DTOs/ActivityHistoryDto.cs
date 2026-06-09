using CRM.Application.Customers.DTOs;

namespace CRM.Application.CRM.Activities.DTOs;

public sealed class ActivityHistoryDto
{
    public string Action { get; init; } = default!;

    public Guid? UserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public bool Succeeded { get; init; }

    public string? FailureReason { get; init; }

    public IReadOnlyList<PropertyChangeDto> Changes { get; init; }
        = [];
}