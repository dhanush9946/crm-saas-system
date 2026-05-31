using CRM.Domain.CRM.Entities;

namespace CRM.Application.CRM.Customers.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(
    Guid tenantId,
    Guid customerId,
    CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default);

    void Update(Customer customer);

    Task<(IReadOnlyList<Customer> Customers, int TotalCount)>
    GetPagedAsync(
        Guid tenantId,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken);


}