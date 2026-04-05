

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
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

        // Generate raw + hash
        public (string rawToken, byte[] hash) Generate()
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = Hash(rawToken);

            return (rawToken, hash);
        }

        // Hash token
        public byte[] Hash(string rawToken)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        }

        // Get token from DB
        public async Task<RefreshToken?> GetByTokenAsync(string rawToken,CancellationToken cancellationToken)
        {
            var hash = Hash(rawToken);
            return await _repository.GetByHashAsync(hash,cancellationToken);
        }

        // CREATE (Used in LOGIN)
        public async Task<(string accessToken, string refreshToken)> CreateAsync(
            Guid tenantId,
            Guid userId,
            string email,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            // 1. Generate token
            var (rawToken, hash) = Generate();

            // 2. Create entity
            var refreshToken = RefreshToken.Create(
                tenantId,
                userId,
                hash,
                DateTime.UtcNow.AddDays(7),
                deviceId,
                userAgent,
                ipAddress
            );

            // 3. Save
            await _repository.AddAsync(refreshToken,cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken); 

            // 4. Generate access token
            var accessToken = _jwtService.GenerateToken(userId, tenantId, email);

            return (accessToken, rawToken);
        }

        //  ROTATE (Used in REFRESH)
        public async Task<AuthResponseDto> RotateAsync(
            RefreshToken existingToken,
            Guid tenantId,
            Guid userId,
            string email,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            if (!existingToken.IsActive())
                throw new UnauthorizedException("Invalid refresh token");

            // 1. Generate new token
            var (newRawToken, newHash) = Generate(); 

            var newToken = RefreshToken.Create(
                tenantId,
                userId,
                newHash,
                DateTime.UtcNow.AddDays(7),
                deviceId,
                userAgent,
                ipAddress
            );

            // 2. Revoke old token
            existingToken.MarkAsReplaced(newToken.Id);

            // 3. Save new token
            await _repository.AddAsync(newToken,cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken); 

            // 4. Generate new access token
            var accessToken = _jwtService.GenerateToken(userId, tenantId, email);

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