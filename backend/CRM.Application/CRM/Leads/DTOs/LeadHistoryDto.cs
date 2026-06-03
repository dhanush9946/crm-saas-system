namespace CRM.Application.CRM.Leads.DTOs;

public sealed class LeadHistoryDto
{
    public string Action { get; init; } = default!;

    public Guid? UserId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public bool Succeeded { get; init; }

    public string? FailureReason { get; init; }

    public IReadOnlyList<LeadPropertyChangeDto> Changes { get; init; }
        = [];
}
