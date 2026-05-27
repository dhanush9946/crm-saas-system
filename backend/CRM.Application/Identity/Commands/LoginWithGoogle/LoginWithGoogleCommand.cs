using MediatR;

namespace CRM.Application.Identity.Commands.LoginWithGoogle;

public sealed class LoginWithGoogleCommand : IRequest<LoginWithGoogleResponse>
{
    public string IdToken { get; set; } = default!;

    public string? DeviceId { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public string? TraceId { get; set; }
}
