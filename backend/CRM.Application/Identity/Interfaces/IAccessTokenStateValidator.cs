namespace CRM.Application.Identity.Interfaces;

public interface IAccessTokenStateValidator
{
    Task<bool> IsValidAsync(
        Guid userId,
        Guid tenantId,
        int tokenVersion,
        CancellationToken cancellationToken);
}