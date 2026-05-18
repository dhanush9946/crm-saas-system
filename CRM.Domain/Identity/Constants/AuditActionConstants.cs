namespace CRM.Domain.Identity.Constants;

public static class AuditActionConstants
{
    public const string TenantRegistered = "TenantRegistered";
    public const string LoginSucceeded = "LoginSucceeded";
    public const string LoginFailed = "LoginFailed";
    public const string LogoutSucceeded = "LogoutSucceeded";
    public const string LogoutAllSucceeded = "LogoutAllSucceeded";
    public const string SessionRevoked = "SessionRevoked";
    public const string RefreshTokenRotated = "RefreshTokenRotated";
    public const string RefreshTokenReuseDetected = "RefreshTokenReuseDetected";
    public const string EmailVerified = "EmailVerified";
    public const string EmailVerificationFailed = "EmailVerificationFailed";
    public const string VerificationEmailResent = "VerificationEmailResent";
}
