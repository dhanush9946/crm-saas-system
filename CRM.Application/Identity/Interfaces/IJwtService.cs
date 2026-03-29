using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, Guid tenantId, string email);
    }
}
