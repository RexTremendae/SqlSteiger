namespace SqlSteiger.CLI;

#pragma warning disable IDE0052 // Private member can be removed as the value assigned to it is never read

public class ExecutionOptionsBuilder(string configFile, string connectionString)
{
    private readonly string _configFile = configFile;
    private readonly string _connectionString = connectionString;

    public ExecutionOptions Build()
    {
        var isValid = false;
        var validationMessage =
            "Connection string must be specified, either in the config file specified by " +
            "the '--config-file' argument or directly via the '--connection-string' argument.";

        return new ExecutionOptions
        {
            IsValid = isValid,
            ValidationMessage = validationMessage,
            ConnectionString = NullableString(_connectionString),
        };
    }

    private static string? NullableString(string value)
    {
        return string.IsNullOrEmpty(value)
            ? null
            : value;
    }
}
