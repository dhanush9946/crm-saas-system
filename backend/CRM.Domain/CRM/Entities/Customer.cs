using CRM.Domain.Common;
using CRM.Domain.CRM.Enums;


namespace CRM.Domain.CRM.Entities;

public sealed class Customer : BaseEntity
{
    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Industry { get; private set; }

    public string? Website { get; private set; }

    public CustomerStatus Status { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public bool IsDeleted { get; private set; }

    private Customer()
    {
    }

    private Customer(
        Guid tenantId,
        string name,
        string? industry,
        string? website,
        Guid? ownerUserId)
    {
        TenantId = tenantId;
        Name = name;
        Industry = industry;
        Website = website;
        OwnerUserId = ownerUserId;

        Status = CustomerStatus.Active;
        IsDeleted = false;
    }

    public static Customer Create(
        Guid tenantId,
        string name,
        string? industry,
        string? website,
        Guid? ownerUserId)
    {
        ValidateName(name);

        return new Customer(
            tenantId,
            name.Trim(),
            industry?.Trim(),
            website?.Trim(),
            ownerUserId);
    }

    public void Update(
        string name,
        string? industry,
        string? website,
        Guid? ownerUserId)
    {
        EnsureNotDeleted();

        ValidateName(name);

        Name = name.Trim();
        Industry = industry?.Trim();
        Website = website?.Trim();
        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void ChangeStatus(CustomerStatus status)
    {
        EnsureNotDeleted();

        Status = status;

        SetUpdated();
    }

    public void Delete()
    {
        EnsureNotDeleted();

        IsDeleted = true;

        SetUpdated();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name is required.");
        }

        if (name.Length > 200)
        {
            throw new ArgumentException(
                "Customer name cannot exceed 200 characters.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Deleted customer cannot be modified.");
        }
    }
}