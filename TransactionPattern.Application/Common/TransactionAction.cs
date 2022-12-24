namespace TransactionPattern.Application.Common;

public abstract class TransactionAction<TContext>
{
    public Result ExecutionResult { get; private set; } = Result.Failed();

    protected abstract Result ExecuteAction(TContext context);
    protected abstract void UndoAction(TContext context);

    public Result Execute(TContext context)
    {
        if (ExecutionResult is not null and { Succeeded: true })
            return ExecutionResult;

        try
        {
            return ExecutionResult = ExecuteAction(context);
        }
        catch (Exception ex)
        {
            return Result.Failed(new ResultError(ex.Message, ex));
        }
    }

    public void Undo(TContext context)
    {
        try
        {
            UndoAction(context);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}

