using System.Security.Claims;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CRM.Infrastructure.Identity;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirstValue("sub");

            return Guid.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : throw new UnauthorizedException("Invalid authentication token");
        }
    }

    public Guid TenantId
    {
        get
        {
            var tenantId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue("tenantId");

            return Guid.TryParse(tenantId, out var parsedTenantId)
                ? parsedTenantId
                : throw new UnauthorizedException("Invalid authentication token");
        }
    }
}
