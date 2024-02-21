namespace SqlSteiger.CLI.CommandLine;

using System.Text;

public static class CommandLineInfo
{
    public static void PrintUsage<T>()
        where T : new()
    {
        var columns = new List<(string Option, string Description)>();
        var optionColumnMaxWidth = 0;
        var columnDistance = 4;

        foreach (var (longName, (attribute, _)) in CommandLineParser.GetAllOptions<T>().OrderBy(_ => _.Key))
        {
            var first = true;
            var builder = new StringBuilder();

            foreach (var shortName in attribute.ShortNames)
            {
                if (!first)
                {
                    builder.Append(", ");
                }

                builder.Append($"-{shortName}");
                first = false;
            }

            if (!first)
            {
                builder.Append(", ");
            }

            builder.Append($"--{longName}");
            var optionString = builder.ToString();
            columns.Add((optionString, attribute.Description));
            optionColumnMaxWidth = int.Max(optionColumnMaxWidth, optionString.Length);
        }

        foreach (var (option, description) in columns)
        {
            ColorWriter.Write(option.PadRight(optionColumnMaxWidth + columnDistance), ConsoleColor.White);
            ColorWriter.WriteLine(description);
        }

        ColorWriter.WriteLine();
    }

    public static void PrintLogo()
    {
        var text = "  SQL Steiger CLI  ";
        var border = string.Empty.PadLeft(text.Length, '═');
        var borderColor = ConsoleColor.White;

        ColorWriter.WriteLine();

        ColorWriter.Write("╔", borderColor);
        ColorWriter.Write(border, borderColor);
        ColorWriter.WriteLine("╗", borderColor);

        ColorWriter.Write("║", borderColor);
        ColorWriter.Write(text);
        ColorWriter.WriteLine("║", borderColor);

        ColorWriter.Write("╚", borderColor);
        ColorWriter.Write(border, borderColor);
        ColorWriter.WriteLine("╝", borderColor);

        ColorWriter.WriteLine();
    }

    public static void SuggestHelp()
    {
        ColorWriter.WriteLine();
        ColorWriter.WriteLine("Use --help to get a complete list of available options.");
        ColorWriter.WriteLine();
    }
}
