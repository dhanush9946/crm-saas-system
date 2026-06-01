namespace CRM.Application.Identity.Commands.LoginWithGoogle;

public sealed class LoginWithGoogleResponse
{
    public bool RequiresOnboarding { get; init; }

    public string? AccessToken { get; init; }

    public string? RefreshToken { get; init; }

    public Guid? UserId { get; init; }

    public Guid? TenantId { get; init; }

    public Guid? SessionId { get; init; }

    public string Email { get; init; } = default!;

    public string FullName { get; init; } = default!;
}
