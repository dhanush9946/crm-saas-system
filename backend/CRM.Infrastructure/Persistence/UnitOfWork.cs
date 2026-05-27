using CRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IApplicationTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);

            return new EfApplicationTransaction(transaction);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private sealed class EfApplicationTransaction : IApplicationTransaction
        {
            private readonly IDbContextTransaction _transaction;

            public EfApplicationTransaction(IDbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                return _transaction.CommitAsync(cancellationToken);
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                return _transaction.RollbackAsync(cancellationToken);
            }

            public ValueTask DisposeAsync()
            {
                return _transaction.DisposeAsync();
            }
        }
    }
}
