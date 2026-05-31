public sealed class CustomerDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Website { get; set; }

    public string Status { get; set; } = string.Empty;

    public Guid? OwnerUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}