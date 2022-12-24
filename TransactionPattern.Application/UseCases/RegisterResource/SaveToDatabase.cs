using TransactionPattern.Application.Common;
using TransactionPattern.Infrastructure;

namespace TransactionPattern.Application.UseCases.RegisterResource;

public class SaveToDatabase : TransactionAction<TransactionContext>
{
    public DataRepository DataRepository { get; }

    public SaveToDatabase(DataRepository dataRepository)
    {
        DataRepository = dataRepository;
    }

    protected override Result ExecuteAction(TransactionContext context)
    {
        if (Random.Shared.Next(10) > 7)
        {
            Console.WriteLine("Saving to database, File: {0} with ApiSerivceId: {1}", context.FilePath, context.SerivceId);
            context.DatabaseId = Guid.NewGuid();
            return Result.Success();
        }
        else
        {
            var ex = new RegisterResourceException("Error saving to database.", RegisterResourceError.Unexpected);
            return Result.Failed(new ResultError(ex.Message, ex));
        }
    }

    protected override void UndoAction(TransactionContext context)
    {

        Console.WriteLine("Removing from database");
        if (Random.Shared.Next(20) > 12)
        {
            throw new Exception("Error undoing Save to database");
        }
        else if (Random.Shared.Next(20) > 7)
        {
            throw new Exception("Error removing database");
        }
    }
}
