namespace SqlSteiger.CLI.CommandLine;

public class CommandLineParserResult<T>(T options, bool isValid, string errorMessage)
    where T : new()
{
    public bool IsValid { get; } = isValid;

    public string ErrorMessage { get; } = errorMessage;

    public T Options { get; } = options;
}
