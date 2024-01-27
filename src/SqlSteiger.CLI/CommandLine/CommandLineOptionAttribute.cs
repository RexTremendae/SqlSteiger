namespace SqlSteiger.CLI.CommandLine;

[AttributeUsage(AttributeTargets.Property)]
public class CommandLineOptionAttribute(string longName = "", string shortName = "") : Attribute
{
    public string LongName => longName;

    public string ShortName => shortName;
}
