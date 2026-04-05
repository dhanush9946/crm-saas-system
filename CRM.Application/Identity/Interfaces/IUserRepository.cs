using CRM.Domain.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Identity.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id,CancellationToken cancellationToken);

        Task<User?> GetByEmailAsync(Guid tenantId, string email,CancellationToken cancellationToken);

        Task AddAsync(User user,CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
