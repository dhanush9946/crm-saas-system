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

    Task<bool> CustomerNameExistsAsync(
    Guid tenantId,
    string name,
    CancellationToken cancellationToken);

    Task<bool> CustomerNameExistsForOtherCustomerAsync(
        Guid tenantId,
        Guid customerId,
        string name,
        CancellationToken cancellationToken);


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