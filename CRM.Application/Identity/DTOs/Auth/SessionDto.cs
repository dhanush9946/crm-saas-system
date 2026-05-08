namespace CRM.Application.Identity.DTOs.Auth
{
    public class SessionDto
    {
        public string Device { get; set; } = "Unknown Device";

        public string? Ip { get; set; }

        public DateTime LastActive { get; set; }

        public bool IsCurrent { get; set; }
    }
}
