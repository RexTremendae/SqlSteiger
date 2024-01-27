namespace SqlSteiger.CLI;

public class ExecutionOptions
{
    public bool IsValid { get; init; }

    public string ValidationMessage { get; init; } = string.Empty;

    public string? ConnectionString { get; init; }
}
