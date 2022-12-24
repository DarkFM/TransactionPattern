using TransactionPattern.Application.UseCases.RegisterResource;
using TransactionPattern.Infrastructure;


var usecase = new RegisterResourceUseCase(new HttpClient(), new FileInfo("test.json"), new DataRepository());
usecase.SetOutputPort(new Presenter());
usecase.Execute();


class Presenter : IOutputPort
{
    public void ApiError(int statusCode)
    {
        Console.WriteLine($"Api Error occured: StatusCode: {statusCode}");
    }

    public void Conflict(string message)
    {
        Console.WriteLine($"Resource Conflict!. {message}");
    }

    public void NotAllowed()
    {
        Console.WriteLine($"Action is not allowed for this user");
    }

    public void Success(TestUseCaseResult result)
    {
        Console.WriteLine(result);
    }

    public void Unexpected(string message)
    {
        Console.WriteLine($"An unexpected error occured. {message}");
    }
}

