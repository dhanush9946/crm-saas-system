namespace CRM.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task<IApplicationTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
