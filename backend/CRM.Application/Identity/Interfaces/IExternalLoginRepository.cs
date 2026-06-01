using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces;

public interface IExternalLoginRepository : IRepository<ExternalLogin>
{
    Task<ExternalLogin?> GetByProviderAsync(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken);
}
