namespace SqlSteiger.Console.CommandLine;

using System.Reflection;

public static class CommandLineParser
{
    public static T Parse<T>(string[] arguments)
        where T : new()
    {
        var propertyNameMappings = GetPropertyNameMappings<T>();
        var options = ExtractArguments<T>(arguments, propertyNameMappings);

        return options;
    }

    private static Dictionary<string, PropertyInfo> GetPropertyNameMappings<T>()
    {
        var nameMappings = new Dictionary<string, PropertyInfo>();

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attribute = property.GetCustomAttribute<CommandLineOptionAttribute>();
            if (attribute == null)
            {
                continue;
            }

            var longName = (string.IsNullOrWhiteSpace(attribute.LongName)
                ? property.Name.ToKebabCase()
                : attribute.LongName)
                .ToLower();
            if (!string.IsNullOrWhiteSpace(longName))
            {
                nameMappings.Add(longName, property);
            }

            var shortName = (string.IsNullOrWhiteSpace(attribute.ShortName)
                ? string.Empty
                : attribute.ShortName)
                .ToLower();
            if (!string.IsNullOrWhiteSpace(shortName))
            {
                nameMappings.Add(shortName, property);
            }
        }

        return nameMappings;
    }

    private static T ExtractArguments<T>(string[] arguments, Dictionary<string, PropertyInfo> propertyNameMappings)
        where T : new()
    {
        var options = new T();

        var argList = new List<string>();

        foreach (var arg in arguments)
        {
            var argParts = arg.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            argList.AddRange(argParts);
        }

        (PropertyInfo? Info, string ArgName) currentProperty = (null, string.Empty);

        foreach (var arg in argList)
        {
            if (arg.StartsWith("-"))
            {
                if (currentProperty.Info != null)
                {
                    throw new InvalidOperationException($"Expected parameter value for option {currentProperty.ArgName}");
                }

                var argName = (arg.Length > 1 && arg[1] == '-')
                    ? arg[2..]
                    : arg[1..];

                if (!propertyNameMappings.TryGetValue(argName, out var mappedProperty))
                {
                    throw new InvalidOperationException($"Invalid option: {arg}");
                }

                if (mappedProperty.PropertyType == typeof(bool))
                {
                    SetValue(mappedProperty, options, string.Empty);
                    currentProperty = (null, string.Empty);
                }
                else
                {
                    currentProperty = (mappedProperty, argName);
                }
            }
            else if (currentProperty.Info != null)
            {
                SetValue(currentProperty.Info, options, arg);
                currentProperty = (null, string.Empty);
            }
            else
            {
                throw new InvalidOperationException($"Invalid option: {arg}");
            }
        }

        return currentProperty.Info != null
            ? throw new InvalidOperationException($"Expected parameter value for option '{currentProperty.ArgName}'")
            : options;
    }

    private static void SetValue<T>(PropertyInfo current, T options, string arg)
    {
        switch (current.PropertyType)
        {
            case var type when type == typeof(bool):
            {
                current.SetValue(options, true);
                break;
            }

            case var type when type == typeof(int):
            {
                if (!int.TryParse(arg, out var value))
                {
                    throw new InvalidOperationException($"Could not parse value as int: '{arg}'");
                }

                current.SetValue(options, value);
                break;
            }

            case var type when type == typeof(long):
            {
                if (!long.TryParse(arg, out var value))
                {
                    throw new InvalidOperationException($"Could not parse value as int: '{arg}'");
                }

                current.SetValue(options, value);
                break;
            }

            case var type when type == typeof(string):
            {
                current.SetValue(options, arg);
                break;
            }

            default:
                throw new InvalidOperationException($"Unsupported property value type: {current.PropertyType.Name}");
        }
    }
}
