

using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Domain.Identity.Entities;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Infrastructure.Services
{
    public class RefreshTokenService:IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        private readonly IJwtService _jwtService;

        public RefreshTokenService(
            IRefreshTokenRepository repository,
            IJwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        // 1. Generate token
        public (string rawToken, byte[] hash) Generate()
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = Hash(rawToken);

            return (rawToken, hash);
        }

        // 2. Hash token (SHA256)
        public byte[] Hash(string rawToken)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        }

        // 3. Find token in DB
        public async Task<RefreshToken?> GetByTokenAsync(string rawToken)
        {
            var hash = Hash(rawToken);
            return await _repository.GetByHashAsync(hash);
        }

        // 4. Rotate token (VERY IMPORTANT)
        public async Task<AuthResponseDto> RotateAsync(
            RefreshToken existingToken,
            Guid tenantId,
            Guid userId,
            string email,
            string? deviceId,
            string? userAgent,
            string? ipAddress)
        {
            if (!existingToken.IsActive())
                throw new UnauthorizedException("Invalid refresh token");

            // Generate new token
            var (newRawToken, newHash) =  GenerateAsync();

            var newToken = RefreshToken.Create(
                tenantId,
                userId,
                newHash,
                DateTime.UtcNow.AddDays(7),
                deviceId,
                userAgent,
                ipAddress
            );

            // Revoke old token
            existingToken.MarkAsReplaced(newToken.Id);

            // Save new token
            await _repository.AddAsync(newToken);

            // Generate new access token
            var accessToken = _jwtService.GenerateToken(userId,tenantId, email);

            return new AuthResponseDto
            {
                TenantId = tenantId,
                UserId = userId,
                AccessToken = accessToken,
                RefreshToken = newRawToken
            };
        }
    }
}
