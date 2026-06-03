using CRM.Domain.Common;
using CRM.Domain.Common.Interfaces;
using CRM.Domain.CRM.Enums;
using CRM.Domain.Leads.Enums;
using System.Net.Mail;

namespace CRM.Domain.CRM.Entities;

public sealed class Lead : BaseEntity, IAuditable
{
    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    public const int MaxEmailLength = 320;
    public const int MaxPhoneLength = 30;
    public const int MaxCompanyLength = 200;
    public const int MaxScoreVersionLength = 50;

    public Guid TenantId { get; private set; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Company { get; private set; }

    public LeadSource Source { get; private set; }

    public LeadStatus Status { get; private set; }

    public decimal? Score { get; private set; }

    public string? ScoreVersion { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public bool IsDeleted { get; private set; }

    private Lead()
    {
    }

    private Lead(
        Guid tenantId,
        string? firstName,
        string? lastName,
        string? email,
        string? phone,
        string? company,
        LeadSource source,
        Guid? ownerUserId)
    {
        TenantId = tenantId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Company = company;
        Source = source;
        OwnerUserId = ownerUserId;

        Status = LeadStatus.New;
        IsDeleted = false;
    }

    public static Lead Create(
        Guid tenantId,
        string? firstName,
        string? lastName,
        string? email,
        string? phone,
        string? company,
        LeadSource source,
        Guid? ownerUserId)
    {
        ValidateTenant(tenantId);
        ValidateContactInformation(email, phone, company);
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateEmail(email);
        ValidatePhone(phone);
        ValidateCompany(company);

        return new Lead(
            tenantId,
            firstName?.Trim(),
            lastName?.Trim(),
            email?.Trim().ToLowerInvariant(),
            phone?.Trim(),
            company?.Trim(),
            source,
            ownerUserId);
    }

    public void Update(
        string? firstName,
        string? lastName,
        string? email,
        string? phone,
        string? company,
        LeadSource source,
        Guid? ownerUserId)
    {
        EnsureNotDeleted();

        ValidateContactInformation(email, phone, company);
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateEmail(email);
        ValidatePhone(phone);
        ValidateCompany(company);

        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        Company = company?.Trim();
        Source = source;
        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void ChangeStatus(LeadStatus status)
    {
        EnsureNotDeleted();

        if (Status == status)
            return;

        Status = status;

        SetUpdated();
    }

    public void AssignOwner(Guid? ownerUserId)
    {
        EnsureNotDeleted();

        if (OwnerUserId == ownerUserId)
            return;

        OwnerUserId = ownerUserId;

        SetUpdated();
    }

    public void UpdateScore(
        decimal score,
        string scoreVersion)
    {
        EnsureNotDeleted();

        if (score < 0 || score > 100)
            throw new ArgumentException(
                "Lead score must be between 0 and 100.");

        if (string.IsNullOrWhiteSpace(scoreVersion))
            throw new ArgumentException(
                "Score version is required.");

        if (scoreVersion.Length > MaxScoreVersionLength)
            throw new ArgumentException(
                $"Score version cannot exceed {MaxScoreVersionLength} characters.");

        Score = score;
        ScoreVersion = scoreVersion.Trim();

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
            throw new ArgumentException(
                "TenantId is required.");
    }

    private static void ValidateContactInformation(
        string? email,
        string? phone,
        string? company)
    {
        if (string.IsNullOrWhiteSpace(email) &&
            string.IsNullOrWhiteSpace(phone) &&
            string.IsNullOrWhiteSpace(company))
        {
            throw new ArgumentException(
                "At least one of Email, Phone, or Company must be provided.");
        }
    }

    private static void ValidateFirstName(string? firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return;

        if (firstName.Length > MaxFirstNameLength)
        {
            throw new ArgumentException(
                $"First name cannot exceed {MaxFirstNameLength} characters.");
        }
    }

    private static void ValidateLastName(string? lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return;

        if (lastName.Length > MaxLastNameLength)
        {
            throw new ArgumentException(
                $"Last name cannot exceed {MaxLastNameLength} characters.");
        }
    }

    private static void ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        if (email.Length > MaxEmailLength)
        {
            throw new ArgumentException(
                $"Email cannot exceed {MaxEmailLength} characters.");
        }

        try
        {
            var mailAddress = new MailAddress(email);

            if (mailAddress.Address != email)
            {
                throw new ArgumentException(
                    "Invalid email address.");
            }
        }
        catch
        {
            throw new ArgumentException(
                "Invalid email address.");
        }


    }

    private static void ValidatePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return;

        if (phone.Length > MaxPhoneLength)
        {
            throw new ArgumentException(
                $"Phone cannot exceed {MaxPhoneLength} characters.");
        }
    }

    private static void ValidateCompany(string? company)
    {
        if (string.IsNullOrWhiteSpace(company))
            return;

        if (company.Length > MaxCompanyLength)
        {
            throw new ArgumentException(
                $"Company cannot exceed {MaxCompanyLength} characters.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Deleted lead cannot be modified.");
        }
    }
}