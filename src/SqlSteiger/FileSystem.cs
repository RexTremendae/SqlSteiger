namespace SqlSteiger;

public class FileSystem : IFileSystem
{
    public Task<string> ReadAllTextAsync(string filepath)
    {
        return File.ReadAllTextAsync(filepath);
    }
}
