namespace CRM.Application.Common.Exceptions;

public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message)
        : base(message)
    {
    }
}