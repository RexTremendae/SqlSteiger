namespace SqlSteiger.CLI;

using System.Text.Json;
using SqlSteiger.CLI.CommandLine;

public class ExecutionOptionsBuilder(IFileSystem fileSystem)
{
    private readonly IFileSystem _fileSystem = fileSystem;

    public async Task<(bool IsSuccess, ExecutionOptions Result)> BuildAsync(CommandLineParserResult<CommandLineOptions> parseResult)
    {
        if (!parseResult.IsValid)
        {
            ColorWriter.WriteLine(parseResult.ErrorMessage, ConsoleColor.Red);
            CommandLineInfo.SuggestHelp();

            return (false, new());
        }

        var executionOptions = new ExecutionOptions();

        if (!string.IsNullOrEmpty(parseResult.Options.ExecutionOptionsFile))
        {
            var optionsFileJsonContent = await _fileSystem.ReadAllTextAsync(parseResult.Options.ExecutionOptionsFile);
            executionOptions = JsonSerializer.Deserialize<ExecutionOptions>(optionsFileJsonContent)
                ?? throw new InvalidOperationException("Could not deserialize specified options file.");
        }

        var mappedConsoleArgOptions = ExecutionOptionsMapper.MapToExecutionOptions(parseResult.Options);

        executionOptions = ExecutionOptionsMapper.MergeExecutionOptions(primary: mappedConsoleArgOptions, secondary: executionOptions);

        if (parseResult.Options.Help)
        {
            CommandLineInfo.PrintUsage<CommandLineOptions>();

            return (false, new());
        }

        var validator = new ExecutionOptionsValidator();
        var validationResult = validator.Validate(executionOptions);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ColorWriter.WriteLine(error, ConsoleColor.Red);
            }

            CommandLineInfo.SuggestHelp();

            return (false, new());
        }

        return (true, executionOptions);
    }
}
