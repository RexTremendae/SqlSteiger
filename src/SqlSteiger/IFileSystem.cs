namespace SqlSteiger;

public interface IFileSystem
{
    Task<string> ReadAllTextAsync(string executionOptionsFile);
}
