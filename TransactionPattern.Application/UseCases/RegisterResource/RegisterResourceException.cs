using TransactionPattern.Application.Common.Exceptions;

namespace TransactionPattern.Application.UseCases.RegisterResource;

public class RegisterResourceException : AppExceptionBase
{
    public RegisterResourceException(string? message, RegisterResourceError errorType, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorType = errorType;
    }

    public RegisterResourceError ErrorType { get; }
}


public enum RegisterResourceError
{
    ApiError,
    Conflict,
    NotAllowed,
    Unexpected
}