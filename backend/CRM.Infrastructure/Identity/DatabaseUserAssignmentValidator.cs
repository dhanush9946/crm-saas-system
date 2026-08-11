using CRM.Application.CRM.Leads.Interfaces;
using CRM.Application.Identity.Interfaces;

namespace CRM.Infrastructure.Identity;

public sealed class DatabaseUserAssignmentValidator
    : IUserAssignmentValidator
{
    private readonly IUserRepository _userRepository;

    public DatabaseUserAssignmentValidator(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> CanAssignAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            tenantId,
            userId,
            cancellationToken);

        return user is not null && !user.IsDisabled();
    }
}