using System.Globalization;

namespace SqlDataExtractor;

public static class ObjectExtensions
{
    public static string ToQueryValue(this object? value)
    {
        if (value == null) return "NULL";

        var type = value.GetType();

        if (type == typeof(bool))
        {
            return (bool)value ? "1" : "0";
        }

        var queryValue = ToString(value, type);

        var quoted = new[] {
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        }.Contains(type);

        return quoted
            ? $"'{queryValue.Replace("'", "''")}'"
            : queryValue;
    }

    private static string ToString(object value, Type type)
    {
        var returnValue = type switch
        {
            var t when t == typeof(float)   => ((float)value).ToString(CultureInfo.InvariantCulture),
            var t when t == typeof(double)  => ((double)value).ToString(CultureInfo.InvariantCulture),
            var t when t == typeof(decimal) => ((decimal)value).ToString(CultureInfo.InvariantCulture),
            _ => value.ToString()
        };

        return returnValue!;
    }
}
