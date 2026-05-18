namespace CRM.Application.Common.Interfaces;

public interface ITokenGenerator
{
    string GenerateSecureToken();

    byte[] ComputeSha256Hash(string input);
}