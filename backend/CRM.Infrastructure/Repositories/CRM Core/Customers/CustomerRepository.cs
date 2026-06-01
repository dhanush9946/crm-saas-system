using CRM.Application.CRM.Customers.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(
            customer,
            cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
    .FirstOrDefaultAsync(
        x => x.Id == customerId &&
             x.TenantId == tenantId,
        cancellationToken);
    }

    //This is for Create customer,checking the customer name is already exists.
    public async Task<bool> CustomerNameExistsAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.Name == name &&
                 !x.IsDeleted,
            cancellationToken);
    }

    //This is for Update customer,checking the customer name exist in the tenant with defferent customer id.
    public async Task<bool> CustomerNameExistsForOtherCustomerAsync(
    Guid tenantId,
    Guid customerId,
    string name,
    CancellationToken cancellationToken = default)
    {
        return await _context.Customers.AnyAsync(
            x => x.TenantId == tenantId
                 && x.Id != customerId
                 && x.Name == name
                 && !x.IsDeleted,
            cancellationToken);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }



    //this method for get Customers query

    public async Task<(IReadOnlyList<Customer> Customers, int TotalCount)>
    GetPagedAsync(
        Guid tenantId,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Customer> query = _context.Customers
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                !x.IsDeleted);

        //---------------------------------
        // Search
        //---------------------------------

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(
                    x.Name,
                    $"%{search}%")
                ||
                (x.Industry != null &&
                 EF.Functions.Like(
                     x.Industry,
                     $"%{search}%"))
                ||
                (x.Website != null &&
                 EF.Functions.Like(
                     x.Website,
                     $"%{search}%")));
                    }

        //---------------------------------
        // Sorting
        //---------------------------------

        var descending =
            string.Equals(
                sortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        query = sortBy?.ToLower() switch
        {
            "name" => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            "createdatutc" => descending
                ? query.OrderByDescending(x => x.CreatedAtUtc)
                : query.OrderBy(x => x.CreatedAtUtc),

            _ => query.OrderByDescending(x => x.CreatedAtUtc)
        };

        //---------------------------------
        // Count
        //---------------------------------

        var totalCount = await query.CountAsync(
            cancellationToken);

        //---------------------------------
        // Paging
        //---------------------------------

        var customers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }


}