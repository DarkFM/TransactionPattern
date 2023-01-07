namespace TransactionPattern.Application.Common;

public interface ITransactionContext<out T> where T : ResultBase
{
    public T GetResult();
}

public abstract class TransactionBase<TContext>
{
    protected TransactionBase(TContext context, IEnumerable<TransactionAction<TContext>> stagedActions)
    {
        Context = context;
        CompletedActions = new Stack<TransactionAction<TContext>>();
        StagedActions = new Queue<TransactionAction<TContext>>(stagedActions);
    }

    protected Stack<TransactionAction<TContext>> CompletedActions { get; }
    protected Queue<TransactionAction<TContext>> StagedActions { get; }
    protected TContext Context { get; }

    public abstract ResultBase Execute();

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

    protected void Undo()
    {
        while (CompletedActions.Any())
            CompletedActions.Pop().Undo(Context);
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
}

public class Transaction<TContext> : TransactionBase<TContext>
{
    public Transaction(TContext context, IEnumerable<TransactionAction<TContext>> actions)
        : base(context, actions)
    {
    }

    public override Result Execute()
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
}

public class Transaction<TContext, TValue> : TransactionBase<TContext>
    where TContext : ITransactionContext<Result<TValue>>
{
    public static Transaction<TContext, TValue> Create(
        TContext context, IEnumerable<TransactionAction<TContext>> actions)
    {
        return new Transaction<TContext, TValue>(context, actions);
    }

    public Transaction(TContext context, IEnumerable<TransactionAction<TContext>> actions) 
        : base(context, actions)
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
