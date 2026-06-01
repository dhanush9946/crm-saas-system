namespace CRM.API.Requests.Auth
{
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// Optional when refresh token is supplied via HttpOnly cookie.
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
