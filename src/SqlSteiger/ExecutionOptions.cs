namespace SqlSteiger;

public class ExecutionOptions(string? connectionString = null)
{
    public string ConnectionString { get; } = connectionString ?? string.Empty;
}
