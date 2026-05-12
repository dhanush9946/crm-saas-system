using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, Guid tenantId, Guid sessionId, string email, int tokenVersion, IEnumerable<string> roles);
    }
}
