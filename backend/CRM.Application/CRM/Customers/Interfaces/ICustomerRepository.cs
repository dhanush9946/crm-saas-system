using CRM.Domain.CRM.Entities;

namespace CRM.Application.CRM.Customers.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default);

    void Update(Customer customer);

   
}