namespace CRM.API.Responses;

/// <summary>
/// Auth payload returned to the browser. Refresh tokens are delivered only via HttpOnly cookie.
/// </summary>
public class AuthPublicResponseDto
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public string AccessToken { get; set; } = null!;
}
