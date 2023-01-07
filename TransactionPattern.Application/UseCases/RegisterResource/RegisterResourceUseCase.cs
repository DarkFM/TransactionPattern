using TransactionPattern.Application.Common;
using TransactionPattern.Infrastructure;

namespace TransactionPattern.Application.UseCases.RegisterResource;

public class RegisterResourceUseCase
{
    private HttpClient httpClient;
    private FileInfo fileInfo;
    private DataRepository dataRepository;

    public RegisterResourceUseCase(HttpClient httpClient, FileInfo fileInfo, DataRepository dataRepository)
    {
        this.httpClient = httpClient;
        this.fileInfo = fileInfo;
        this.dataRepository = dataRepository;
    }

    private IOutputPort OutputPort { get; set; }

    public void SetOutputPort(IOutputPort outputPort) => OutputPort = outputPort;

    public void Execute()
    {
        var registerGssAction = new RegisterWithApi(httpClient);
        var saveToDiskAction = new SaveToDisk(fileInfo);
        var saveToDbAction = new SaveToDatabase(dataRepository);

        var actions = new TransactionAction<TransactionContext>[] { saveToDiskAction, registerGssAction, saveToDbAction };

        var transaction = Transaction<TransactionContext, TestUseCaseResult>.Create(new TransactionContext(), actions);
        var result = transaction.Execute();

        if (result.Succeeded)
        {
            OutputPort.Success(result.Value!);
        }
        else
        {
            var error = result.Errors.Single();
            if (error.Exception is RegisterResourceException ex)
            {
                switch (ex.ErrorType)
                {
                    case RegisterResourceError.ApiError:
                        OutputPort.ApiError(409); // Ideally we will pass the Api status code down to this
                        break;
                    case RegisterResourceError.Conflict:
                        OutputPort.Conflict(ex.Message);
                        break;
                    case RegisterResourceError.NotAllowed:
                        OutputPort.NotAllowed();
                        break;
                    case RegisterResourceError.Unexpected:
                    default:
                        OutputPort.Unexpected(ex.Message);
                        break;
                }
            }
            else
            {
                OutputPort.Unexpected(error.Message);
            }
        }
    }
}

public record TestUseCaseResult(Guid ApiServiceId, string ResourcePath, Guid ResourceId);

public record TransactionContext : ITransactionContext<TestUseCaseResult>
{
    public Guid SerivceId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Guid DatabaseId { get; set; }

    public Result<TestUseCaseResult> GetResult()
    {
        return Result<TestUseCaseResult>.Success(new TestUseCaseResult(SerivceId, "resource://" + FilePath, DatabaseId));
    }
}
