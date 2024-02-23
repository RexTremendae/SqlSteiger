namespace SqlSteiger.CLI;

public static class ColorWriter
{
    public static void WriteLine(object? data = null, ConsoleColor? color = null)
    {
        if (color == null)
        {
            Console.WriteLine(data);
            return;
        }

        Console.ForegroundColor = color.Value;
        Console.WriteLine(data);
        Console.ResetColor();
    }

    public static void Write(object? data = null, ConsoleColor? color = null)
    {
        if (color == null)
        {
            Console.Write(data);
            return;
        }

        Console.ForegroundColor = color.Value;
        Console.Write(data);
        Console.ResetColor();
    }
}
