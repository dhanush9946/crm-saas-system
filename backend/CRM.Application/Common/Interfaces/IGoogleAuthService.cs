using CRM.Application.Common.Models;

namespace CRM.Application.Common.Interfaces;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ValidateAsync(
        string idToken,
        CancellationToken cancellationToken = default);
}