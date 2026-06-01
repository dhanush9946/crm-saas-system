using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;

using Google.Apis.Auth;

using Microsoft.Extensions.Options;

namespace CRM.Infrastructure.Identity.Google;

public sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleAuthOptions _options;

    public GoogleAuthService(
        IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GoogleUserInfo> ValidateAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new ArgumentException("Id token is required.");

        var validationSettings =
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_options.ClientId]
            };

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            validationSettings);

        return new GoogleUserInfo
        {
            Subject = payload.Subject,
            Email = payload.Email,
            Name = payload.Name,
            PictureUrl = payload.Picture,
            EmailVerified = payload.EmailVerified
        };
    }
}