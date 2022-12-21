using TransactionPattern.Application.Common;
using TransactionPattern.Infrastructure;

namespace TransactionPattern.Application.UseCases.ResgisterResource;

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

    public TestUseCaseResult? Execute()
    {
        var registerGssAction = new RegisterWithApi(httpClient);
        var saveToDiskAction = new SaveToDisk(fileInfo);
        var saveToDbAction = new SaveToDatabase(dataRepository);

        var actions = new TransactionAction<TransactionContext>[] { saveToDiskAction, registerGssAction, saveToDbAction };

        var transaction = Transaction<TransactionContext, TestUseCaseResult>.Create(new TransactionContext(), actions);
        var result = transaction.Execute();

        if (result.Succeeded)
        {
            return result.Value;
        }

        throw new Exception(result.Errors.Aggregate("", (s, v) => s + Environment.NewLine + v.Message));
    }
}

public record TestUseCaseResult(Guid ApiServiceId, string ResourcePath, Guid ResourceId);

public record TransactionContext : ITransactionContext<Result<TestUseCaseResult>>
{
    public Guid SerivceId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Guid DatabaseId { get; set; }

    public Result<TestUseCaseResult> GetResult()
    {
        return Result<TestUseCaseResult>.Success(new TestUseCaseResult(SerivceId, "resource://" + FilePath, DatabaseId));
    }
}
