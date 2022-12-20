using TransactionPattern.Application.Common;

namespace TransactionPattern.Application.UseCases.ResgisterResource;

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
            context.GssId = Guid.NewGuid();
            ApiServiceId = context.GssId;

            Console.WriteLine("Successful service registration: " + ApiServiceId);
            return Result.Success();
        }
        else
        {
            return Result.Failed(new ResultError("Error Registering with GSS"));
        }
    }

    protected override Result UndoAction(TransactionContext context)
    {
        Console.WriteLine("Removing " + ApiServiceId);
        if (Random.Shared.Next(10) > 5)
        {
            return Result.Success();
        }
        else
        {
            return Result.Failed(new ResultError("Error unregistering with api service"));
        }
    }
}
