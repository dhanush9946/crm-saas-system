namespace CRM.API.Requests.Auth
{
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = null!;

        public string? DeviceId { get; set; }
    }
}
