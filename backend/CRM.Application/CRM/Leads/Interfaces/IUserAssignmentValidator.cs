namespace CRM.Application.CRM.Leads.Interfaces;

public interface IUserAssignmentValidator
{
    Task<bool> CanAssignAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken);
}