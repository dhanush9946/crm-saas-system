using CRM.Domain.Common;
using CRM.Domain.CRM.Enums;

namespace CRM.Domain.CRM.Entities;
public sealed class Customer : BaseEntity
{
    public const int MaxNameLength = 200;

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
        ValidateTenant(tenantId);
        ValidateName(name);
        ValidateWebsite(website);

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
        ValidateWebsite(website);

        Name = name.Trim();
        Industry = industry?.Trim();
        Website = website?.Trim();
        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void ChangeStatus(CustomerStatus status)
    {
        EnsureNotDeleted();

        if (Status == status)
            return;

        Status = status;

        SetUpdated();
    }

    public void SoftDelete()
    {
        EnsureNotDeleted();

        IsDeleted = true;

        SetUpdated();
    }

    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;

        SetUpdated();
    }

    private static void ValidateTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.");

        if (name.Length > MaxNameLength)
            throw new ArgumentException(
                $"Customer name cannot exceed {MaxNameLength} characters.");
    }

    private static void ValidateWebsite(string? website)
    {
        if (string.IsNullOrWhiteSpace(website))
            return;

        if (!Uri.TryCreate(
                website,
                UriKind.Absolute,
                out _))
        {
            throw new ArgumentException("Invalid website url.");
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