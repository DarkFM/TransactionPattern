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
        Actions = new(actions);
    }

    protected TContext Context { get; }
    protected LinkedList<TransactionAction<TContext>> Actions { get; }

    public virtual Result Execute()
    {
        var (lastnode, isSuccess) = ExecuteActions();
        if (isSuccess is false)
        {
            Undo(lastnode.Previous);
            return Result.Failed(CollectError(lastnode));
        }
        else
        {
            return Result.Success();
        }
    }

    protected (LinkedListNode<TransactionAction<TContext>>? LastNode, bool IsSuccess) ExecuteActions()
    {
        LinkedListNode<TransactionAction<TContext>>? currentNode = Actions.First;
        LinkedListNode<TransactionAction<TContext>>? previousNode = currentNode;

        while (currentNode != null)
        {
            previousNode = currentNode;
            if (HandleAction(currentNode.Value) is false)
                return (currentNode, false);

            currentNode = currentNode.Next;
        }

        // if successful completed, then this will be the last node
        return (previousNode, true);
    }

    protected static ResultError[] CollectError(LinkedListNode<TransactionAction<TContext>>? node)
    {
        var errors = new List<ResultError>();
        while (node != null)
        {
            errors.AddRange(node.Value.ExecutionResult.Errors);
            node = node.Previous;
        }

        return errors.ToArray();
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

    protected void Undo(LinkedListNode<TransactionAction<TContext>>? node)
    {
        while (node != null)
        {
            node.Value.Undo(Context);
            node = node.Previous;
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

    public override Result<TValue> Execute()
    {
        var (lastnode, isSuccess) = ExecuteActions();
        if (isSuccess is false)
        {
            Undo(lastnode.Previous);
            return Result<TValue>.Failed(CollectError(lastnode));
        }
        else
        {
            return Context.GetResult();
        }
    }
}

