using System.Security.Cryptography;
using System.Text;
using CRM.Application.Common.Interfaces;

namespace CRM.Infrastructure.Services;

public sealed class TokenGenerator : ITokenGenerator
{
    public string GenerateSecureToken()
    {
        // 32 bytes (256 bits) is standard and very secure for verification tokens
        var bytes = RandomNumberGenerator.GetBytes(32);

        // Convert to URL-safe Base64 string
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public byte[] ComputeSha256Hash(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty", nameof(input));

        // Return byte[] directly to match our EmailVerificationToken.TokenHash property
        return SHA256.HashData(Encoding.UTF8.GetBytes(input));
    }
}