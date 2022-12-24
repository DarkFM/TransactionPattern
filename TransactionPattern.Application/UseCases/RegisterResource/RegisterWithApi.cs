using TransactionPattern.Application.Common;

namespace TransactionPattern.Application.UseCases.RegisterResource;

public class RegisterWithApi : TransactionAction<TransactionContext>
{
    private readonly HttpClient _httpClient;

    public Guid ApiServiceId { get; private set; }

    public RegisterWithApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected override Result ExecuteAction(TransactionContext context)
    {
        if (Random.Shared.Next(10) > 5)
        {
            context.SerivceId = Guid.NewGuid();
            ApiServiceId = context.SerivceId;

            Console.WriteLine("Successful service registration: " + ApiServiceId);
            return Result.Success();
        }
        else
        {
            var ex = new RegisterResourceException("Error Registering with ApiSerivce", RegisterResourceError.ApiError);
            return Result.Failed(new ResultError(ex.Message, ex));
        }
    }

    protected override void UndoAction(TransactionContext context)
    {
        Console.WriteLine("UnRegistering resource from api: " + ApiServiceId);
    }
}
