namespace CRM.API.Requests.Auth
{
    public class VerifyEmailRequestDto
    {
        public string Token { get; set; } = null!;
        public string? DeviceId { get; set; }
    }
}
