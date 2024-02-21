namespace SqlSteiger.CLI;

using SqlSteiger.CLI.CommandLine;

public class CommandLineOptions
{
    [CommandLineOption(
        description: "Set connection string to use for data extraction.",
        parameterName: "connection string")]
    public string? ConnectionString { get; init; }

    [CommandLineOption(
        description: "Specify JSON file that contains execution options.",
        parameterName: "file path")]
    public string? ExecutionOptionsFile { get; init; }

    [CommandLineOption(
        description: "Display full help.",
        shortNames: ["h", "?"])]
    public bool Help { get; init; }

    [CommandLineOption(
        description: "Suppress logo output.")]
    public bool NoLogo { get; init; }
}
