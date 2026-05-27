using MediatR;

namespace CRM.Application.Identity.Commands.CompleteGoogleOnboarding
{
    public class CompleteGoogleOnboardingCommand
        : IRequest<CompleteGoogleOnboardingResponse>
    {
        public string IdToken { get; set; } = null!;

        public string TenantName { get; set; } = null!;

        public string TenantSlug { get; set; } = null!;

        public string? DeviceId { get; set; }

        public string? UserAgent { get; set; }

        public string? IpAddress { get; set; }

        public string? TraceId { get; set; }
    }
}
