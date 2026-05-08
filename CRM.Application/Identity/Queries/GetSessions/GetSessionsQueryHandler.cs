using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using MediatR;

namespace CRM.Application.Identity.Queries.GetSessions
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<SessionDto>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public GetSessionsQueryHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<List<SessionDto>> Handle(
            GetSessionsQuery request,
            CancellationToken cancellationToken)
        {
            var activeTokens = await _refreshTokenRepository.GetActiveByUserAsync(
                request.TenantId,
                request.UserId,
                cancellationToken);

            return activeTokens
                .Select(token => new SessionDto
                {
                    Device = FormatDevice(token.UserAgent),
                    Ip = token.IpAddress,
                    LastActive = token.IssuedAtUtc,
                    IsCurrent = !string.IsNullOrWhiteSpace(request.DeviceId) &&
                                token.DeviceId == request.DeviceId
                })
                .ToList();
        }

        private static string FormatDevice(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown Device";

            var browser = GetBrowser(userAgent);
            var operatingSystem = GetOperatingSystem(userAgent);

            return $"{browser} - {operatingSystem}";
        }

        private static string GetBrowser(string userAgent)
        {
            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
                return "Edge";

            if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
                return "Chrome";

            if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
                return "Firefox";

            if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
                return "Safari";

            return "Unknown Browser";
        }

        private static string GetOperatingSystem(string userAgent)
        {
            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "Windows";

            if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "iPadOS";

            if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                return "iOS";

            if (userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
                return "macOS";

            if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Android";

            if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                return "Linux";

            return "Unknown OS";
        }
    }
}
