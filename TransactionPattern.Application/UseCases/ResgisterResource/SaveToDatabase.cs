using TransactionPattern.Application.Common;
using TransactionPattern.Infrastructure;

namespace TransactionPattern.Application.UseCases.ResgisterResource;

public class SaveToDatabase : TransactionAction<TransactionContext>
{
    public DataRepository DataRepository { get; }

    public SaveToDatabase(DataRepository dataRepository)
    {
        DataRepository = dataRepository;
    }

    protected override Result ExecuteAction(TransactionContext context)
    {
        if (Random.Shared.Next(10) > 8)
        {
            Console.WriteLine("Saving to database, File: {0} with ApiSerivceId: {1}", context.FilePath, context.SerivceId);
            context.DatabaseId = Guid.NewGuid();
            return Result.Success();
        }
        else
        {
            return Result.Failed(new ResultError("Error saving to database."));
        }
    }

    protected override Result UndoAction(TransactionContext context)
    {

        Console.WriteLine("Removing from database");
        if (Random.Shared.Next(20) > 14)
        {
            return Result.Success();
        }
        else if (Random.Shared.Next(20) > 8)
        {
            throw new Exception("Error undoing Save to database");
        }
        else
        {
            return Result.Failed(new ResultError("Error removing database"));
        }
    }
}
