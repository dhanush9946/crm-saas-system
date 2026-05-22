using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRM.Application.Identity.Interfaces;

namespace CRM.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(
            Guid userId,
            Guid tenantId,
            Guid sessionId,
            string email,
            int tokenVersion,
            IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),   
                new Claim("tenantId", tenantId.ToString()),
                new Claim("sessionId", sessionId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("ver", tokenVersion.ToString())
            };

            if (roles != null)
            {
                claims.AddRange(
                    roles.Select(role => new Claim(ClaimTypes.Role, role))
                );
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
