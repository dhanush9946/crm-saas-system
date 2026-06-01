namespace CRM.API.Helpers;

/// <summary>
/// Manages the HttpOnly refresh-token cookie used by the SPA.
/// The raw token is never intended for JavaScript access.
/// </summary>
public static class RefreshTokenCookieHelper
{
    public const string CookieName = "crm_refresh_token";
    public const string DefaultCookiePath = "/api/v1/auth";

    public static void Set(HttpResponse response, string refreshToken, int expirationDays, IConfiguration configuration)
    {
        var cookieOptions = BuildCookieOptions(expirationDays, configuration);
        response.Cookies.Append(CookieName, refreshToken, cookieOptions);
    }

    public static void Clear(HttpResponse response, IConfiguration configuration)
    {
        var cookieOptions = BuildCookieOptions(expirationDays: 0, configuration);
        cookieOptions.Expires = DateTimeOffset.UnixEpoch;
        response.Cookies.Delete(CookieName, cookieOptions);
    }

    public static string? Get(HttpRequest request)
    {
        return request.Cookies.TryGetValue(CookieName, out var token) ? token : null;
    }

    private static CookieOptions BuildCookieOptions(int expirationDays, IConfiguration configuration)
    {
        var path = configuration["Auth:RefreshTokenCookiePath"] ?? DefaultCookiePath;
        var sameSite = ParseSameSite(configuration["Auth:RefreshTokenCookieSameSite"]);
        var secure = configuration.GetValue("Auth:RefreshTokenCookieSecure", true);

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = sameSite,
            Path = path,
            IsEssential = true,
            Expires = expirationDays > 0
                ? DateTimeOffset.UtcNow.AddDays(expirationDays)
                : DateTimeOffset.UtcNow.AddDays(7),
        };
    }

    private static SameSiteMode ParseSameSite(string? value) =>
        value?.ToLowerInvariant() switch
        {
            "none" => SameSiteMode.None,
            "strict" => SameSiteMode.Strict,
            _ => SameSiteMode.Lax,
        };
}
