using System.Collections.Immutable;

namespace TransactionPattern.Infrastructure;

public class DataRepository
{
    private readonly ICollection<string> _installedFiles;
    public ICollection<string> InstalledFiles => _installedFiles.ToImmutableArray();

    public DataRepository(params string[] installedFiles)
    {
        _installedFiles = installedFiles;
    }

    public bool AddFile(string file)
    {
        InstalledFiles.Add(file);
        return true;
    }

    public bool RemoveFile(string file)
    {
        InstalledFiles.Remove(file);
        return true;
    }
}