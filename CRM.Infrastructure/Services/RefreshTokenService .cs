

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        private readonly IConfiguration _config;

        public RefreshTokenService(
            IRefreshTokenRepository repository,
            IConfiguration config)
        {
            _repository = repository;
            _config = config;
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

        // CREATE
        public async Task<string> CreateAsync(
            Guid tenantId,
            Guid userId,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            // 1. Generate token
            var (rawToken, hash) = Generate();

            // 2. Create entity
            var expirationDays = Convert.ToDouble(_config["Jwt:RefreshTokenExpirationDays"] ?? "7");
            var refreshToken = RefreshToken.Create(
                tenantId,
                userId,
                hash,
                DateTime.UtcNow.AddDays(expirationDays),
                deviceId,
                userAgent,
                ipAddress
            );

            // 3. Save
            await _repository.AddAsync(refreshToken,cancellationToken);

            return rawToken;
        }

        //  ROTATE (Used in REFRESH)
        public async Task<string> RotateAsync(
            RefreshToken existingToken,
            Guid tenantId,
            Guid userId,
            string? deviceId,
            string? userAgent,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            if (!existingToken.IsActive())
                throw new UnauthorizedException("Invalid refresh token");

            // 1. Generate new token
            var (newRawToken, newHash) = Generate(); 

            var expirationDays = Convert.ToDouble(_config["Jwt:RefreshTokenExpirationDays"] ?? "7");
            var newToken = RefreshToken.CreateInFamily(
                tenantId,
                userId,
                newHash,
                existingToken.TokenFamilyId,
                DateTime.UtcNow.AddDays(expirationDays),
                deviceId,
                userAgent,
                ipAddress
            );

            // 2. Revoke old token
            existingToken.MarkAsReplaced(newToken.Id);

            // 3. Save new token
            await _repository.AddAsync(newToken,cancellationToken);

            return newRawToken;
        }
    }
}
