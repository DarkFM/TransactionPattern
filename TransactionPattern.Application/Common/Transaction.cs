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
        StagedActions = new(actions);
        CompletedActions= new();
    }

    protected TContext Context { get; }
    protected Queue<TransactionAction<TContext>> StagedActions { get; }
    protected Stack<TransactionAction<TContext>> CompletedActions { get; }

    public virtual Result Execute()
    {
        ExecuteActions();
        if (StagedActions.Any())
        {
            Undo();
            return Result.Failed(StagedActions.Dequeue().ExecutionResult.Errors.ToArray());
        }
        else
        {
            return Result.Success();
        }
    }

    protected void ExecuteActions()
    {
        while (StagedActions.Any())
        {
            var action = StagedActions.Peek();
            if (HandleAction(action) is false)
                break;
            
            CompletedActions.Push(StagedActions.Dequeue());
        }
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

    protected void Undo()
    {
        while (CompletedActions.Any())
            CompletedActions.Pop().Undo(Context);
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

    public override Result<TValue> Execute()
    {
        ExecuteActions();
        if (StagedActions.Any())
        {
            Undo();
            return Result<TValue>.Failed(StagedActions.Dequeue().ExecutionResult.Errors.ToArray());
        }
        else
        {
            return Context.GetResult();
        }
    }
}
