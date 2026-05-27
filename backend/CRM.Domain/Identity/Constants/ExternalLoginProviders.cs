namespace CRM.Domain.Identity.Constants;

public static class ExternalLoginProviders
{
    public const string Google = "GOOGLE";

    public const string Microsoft = "MICROSOFT";

    public const string GitHub = "GITHUB";

    public const string Facebook = "FACEBOOK";

    public static readonly HashSet<string> SupportedProviders =
    [
        Google,
        Microsoft,
        GitHub,
        Facebook
    ];
}