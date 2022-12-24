using TransactionPattern.Application.Common;

namespace TransactionPattern.Application.UseCases.RegisterResource;

public class SaveToDisk : TransactionAction<TransactionContext>
{
    public FileInfo FileInfo { get; }

    public SaveToDisk(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    protected override Result ExecuteAction(TransactionContext context)
    {
        if (Random.Shared.Next(10) > 4)
        {
            Console.WriteLine("Moving File to correct location");
            context.FilePath = FileInfo.FullName;
            return Result.Success();
        }
        else
        {
            var ex = new RegisterResourceException("Error saving to disk. Permission denied", RegisterResourceError.NotAllowed);
            return Result.Failed(new ResultError(ex.Message, ex));
        }
    }

    protected override void UndoAction(TransactionContext context)
    {
        Console.WriteLine("Moving file back into place");
        if (Random.Shared.Next(10) <= 5)
        {
            throw new Exception("Error moving file back to original location");
        }
    }
}
