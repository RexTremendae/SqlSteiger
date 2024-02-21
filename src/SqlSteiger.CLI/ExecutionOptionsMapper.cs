namespace SqlSteiger.CLI;

public static class ExecutionOptionsMapper
{
    public static ExecutionOptions MapToExecutionOptions(CommandLineOptions commandLineOptions)
    {
        return new ExecutionOptions(
            connectionString: commandLineOptions.ConnectionString ?? string.Empty);
    }

    public static ExecutionOptions MergeExecutionOptions(ExecutionOptions primary, ExecutionOptions secondary)
    {
        return new ExecutionOptions(
            connectionString: Coalesce(primary.ConnectionString, secondary.ConnectionString));
    }

    private static string Coalesce(params string?[] inputs)
    {
        foreach (var current in inputs)
        {
            if (!string.IsNullOrEmpty(current))
            {
                return current;
            }
        }

        return string.Empty;
    }
}
