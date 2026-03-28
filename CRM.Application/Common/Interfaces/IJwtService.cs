
using CRM.Domain.Identity.Entities;

namespace CRM.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, Guid tenantId, string email);
    }
}
