namespace CRM.API.Requests.Auth
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = null!;

        public string? DeviceId { get; set; }
    }
}
