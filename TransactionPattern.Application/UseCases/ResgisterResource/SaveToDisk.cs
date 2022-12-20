using TransactionPattern.Application.Common;

namespace TransactionPattern.Application.UseCases.ResgisterResource;

public class SaveToDisk : TransactionAction<TransactionContext>
{
    public FileInfo FileInfo { get; }

    public SaveToDisk(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    protected override Result ExecuteAction(TransactionContext context)
    {
        if (Random.Shared.Next(10) > 5)
        {
            Console.WriteLine("Moving File to correct location");
            context.FilePath = FileInfo.FullName;
            return Result.Success();
        }
        else
        {
            return Result.Failed(new ResultError("Error saving to disk. Permission denied"));
        }
    }

    protected override Result UndoAction(TransactionContext context)
    {
        Console.WriteLine("Moving file back into place");
        if (Random.Shared.Next(10) > 5)
        {
            return Result.Success();
        }
        else
        {
            return Result.Failed(new ResultError("Error moving file back to original location"));
        }
    }
}
