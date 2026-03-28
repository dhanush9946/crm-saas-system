
using CRM.Domain.Identity.Entities;

namespace CRM.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, Tenant tenant);
    }
}
