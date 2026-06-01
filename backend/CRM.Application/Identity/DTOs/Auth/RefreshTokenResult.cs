namespace CRM.Application.Identity.DTOs.Auth
{
    public sealed class RefreshTokenResult
    {
        public Guid SessionId { get; set; }
        public string RawToken { get; set; } = null!;
    }
}
