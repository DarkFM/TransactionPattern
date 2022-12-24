namespace TransactionPattern.Application.UseCases.RegisterResource;

public interface IOutputPort
{
    void Success(TestUseCaseResult result);
    void Conflict(string message);
    void Unexpected(string message);
    void NotAllowed();
    void ApiError(int statusCode);
}
