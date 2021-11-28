namespace SqlDataExtractor;

public static class ObjectExtensions
{
    public static string ToQueryValue(this object? value, Type csharpType)
    {
        if (value == null) return "NULL";

        if (csharpType == typeof(bool))
        {
            return (bool)value ? "1" : "0";
        }

        var queryValue = value.ToString() ?? string.Empty;

        var quoted = new[] {
            typeof(string),
            typeof(DateTime),
            typeof(Guid)
        }.Contains(csharpType);

        return quoted
            ? $"'{queryValue.Replace("'", "''")}'"
            : queryValue;
    }
}
