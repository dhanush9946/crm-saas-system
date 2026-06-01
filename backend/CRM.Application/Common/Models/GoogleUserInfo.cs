namespace CRM.Application.Common.Models;

public sealed class GoogleUserInfo
{
    public string Subject { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Name { get; init; } = default!;

    public string? PictureUrl { get; init; }

    public bool EmailVerified { get; init; }
}