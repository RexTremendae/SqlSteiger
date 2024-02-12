namespace SqlSteiger.CLI.CommandLine;

[AttributeUsage(AttributeTargets.Property)]
public class CommandLineOptionAttribute(string longName = "", string[]? shortNames = null, string? description = null) : Attribute
{
    public CommandLineOptionAttribute(string shortName, string longName = "", string? description = null)
        : this(longName: longName, shortNames: [shortName], description: description)
    {
    }

    public string Description => description ?? string.Empty;

    public string LongName => longName;

    public string[] ShortNames => shortNames ?? [];
}
