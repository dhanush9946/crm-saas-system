using CRM.Domain.Common;
using CRM.Domain.Identity.Constants;

namespace CRM.Domain.Identity.Entities;

public sealed class ExternalLogin : BaseEntity
{
    public Guid TenantId { get; private set; }

    public Guid UserId { get; private set; }

    public string Provider { get; private set; } = default!;

    // Stable external provider unique identifier
    // Example: Google "sub" claim
    public string ProviderUserId { get; private set; } = default!;

    public string? Email { get; private set; }

    // Navigation Property
    public User User { get; private set; } = default!;

    // EF Core
    private ExternalLogin()
    {
    }

    private ExternalLogin(
        Guid tenantId,
        Guid userId,
        string provider,
        string providerUserId,
        string? email)
    {
        TenantId = tenantId;

        UserId = userId;

        Provider = provider;

        ProviderUserId = providerUserId;

        Email = email;
    }

    // Factory Method
    public static ExternalLogin Create(
        Guid tenantId,
        Guid userId,
        string provider,
        string providerUserId,
        string? email = null)
    {
        Validate(tenantId, userId, provider, providerUserId);

        provider = provider.Trim().ToUpperInvariant();

        return new ExternalLogin(
            tenantId,
            userId,
            provider,
            providerUserId.Trim(),
            NormalizeEmail(email));
    }

    // Domain Method
    public void ChangeEmail(string? email)
    {
        Email = NormalizeEmail(email);

        SetUpdated();
    }

    public bool IsProvider(string provider)
    {
        return Provider.Equals(
            provider.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    private static void Validate(
        Guid tenantId,
        Guid userId,
        string provider,
        string providerUserId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.");

        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider is required.");

        provider = provider.Trim().ToUpperInvariant();

        if (!ExternalLoginProviders.SupportedProviders.Contains(provider))
            throw new ArgumentException($"Unsupported provider: {provider}");

        if (string.IsNullOrWhiteSpace(providerUserId))
            throw new ArgumentException("ProviderUserId is required.");
    }

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return email.Trim().ToLowerInvariant();
    }
}