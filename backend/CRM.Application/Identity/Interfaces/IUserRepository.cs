using CRM.Domain.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Identity.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(Guid tenantId, string email,CancellationToken cancellationToken);
        Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        Task<User?> GetByIdAsync(
            Guid tenantId,
            Guid userId,
            CancellationToken cancellationToken);
    }
}
