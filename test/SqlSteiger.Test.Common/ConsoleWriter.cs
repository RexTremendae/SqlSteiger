namespace SqlSteiger.Test.Common;

using Xunit.Abstractions;

public class ConsoleWriter(ITestOutputHelper output) : StringWriter
{
    private static ConsoleWriter? _consoleWriter;
    private readonly ITestOutputHelper _output = output;

    public static void Attach(ITestOutputHelper output)
    {
        _consoleWriter = new(output);
        Console.SetOut(_consoleWriter);
    }

    public override void WriteLine(string? m)
    {
        _output.WriteLine(m);
    }
}
