namespace TransactionPattern.Application.Common.Exceptions;

public class AppExceptionBase : Exception
{
    public AppExceptionBase(string? message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
