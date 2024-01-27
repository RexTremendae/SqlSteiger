namespace SqlSteiger.CLI;

using SqlSteiger.CLI.CommandLine;

public class CommandLineOptions
{
    [CommandLineOption]
    public string ConnectionString { get; set; } = string.Empty;
}
