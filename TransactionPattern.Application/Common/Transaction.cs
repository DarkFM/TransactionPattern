namespace TransactionPattern.Application.Common;

public interface ITransactionContext<out T> where T : Result
{
    public T GetResult();
}

public class Transaction<TContext>
{
    public Transaction(TContext context, IEnumerable<TransactionAction<TContext>> actions)
    {
        Context = context;
        Actions = actions;
    }

    protected TContext Context { get; }
    protected IEnumerable<TransactionAction<TContext>> Actions { get; }

    public virtual Result Execute()
    {
        var actions = Actions.ToArray();
        for (int i = 0; i < actions.Length; i++)
        {
            if (HandleAction(actions[i]) is false)
            {
                Undo(Math.Max(0, i - 1));
                var errors = actions.SelectMany(a => a.ExecutionResult.Errors).ToArray();
                return Result.Failed(errors);
            }
        }

        return Result.Success();
    }

    private bool HandleAction(TransactionAction<TContext> transactionAction)
    {
        int retryCount = 3;
        Result result;
        do
        {
            Console.WriteLine($"Attempt {4 - retryCount} for {transactionAction.GetType().Name}");
            result = transactionAction.Execute(Context);

        } while (result.Succeeded is false && --retryCount >= 0);

        return result.Succeeded;
    }

    private void Undo(int index)
    {
        var actions = Actions.ToArray();
        for (; index >= 0; index--)
        {
            actions[index].Undo(Context);
        }
    }
}

public class Transaction<TContext, TValue> : Transaction<TContext>
    where TContext : ITransactionContext<Result<TValue>>
{

    public static Transaction<TContext, TValue> Create(
        TContext context, IEnumerable<TransactionAction<TContext>> actions)
    {
        return new Transaction<TContext, TValue>(context, actions);
    }

    public Transaction(TContext context, IEnumerable<TransactionAction<TContext>> actions) : base(context, actions)
    {
    }

    public Result<TValue> Execute()
    {
        var actions = Actions.ToArray();
        for (int i = 0; i < actions.Length; i++)
        {
            if (HandleAction(actions[i]) is false)
            {
                Undo(Math.Max(0, i - 1));
                var errors = actions.SelectMany(a => a.ExecutionResult.Errors).ToArray();
                return Result<TValue>.Failed(errors);
            }
        }

        return Context.GetResult();
    }

    private bool HandleAction(TransactionAction<TContext> transactionAction)
    {
        int retryCount = 3;
        Result result;
        do
        {
            Console.WriteLine($"Attempt {4 - retryCount} for {transactionAction.GetType().Name}");
            result = transactionAction.Execute(Context);

        } while (result.Succeeded is false && --retryCount >= 0);

        return result.Succeeded;
    }

    private void Undo(int index)
    {
        var actions = Actions.ToArray();
        for (; index >= 0; index--)
        {
            actions[index].Undo(Context);
        }
    }
}


public abstract class TransactionAction<TContext>
{
    public Result ExecutionResult { get; private set; } = Result.Failed();

    protected abstract Result ExecuteAction(TContext context);
    protected abstract Result UndoAction(TContext context);

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
        catch (Exception)
        {
        }
    }
}

