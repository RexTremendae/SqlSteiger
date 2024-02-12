namespace SqlSteiger.CLI.CommandLine;

using System.Reflection;

public static class CommandLineParser
{
    public static CommandLineParserResult<T> Parse<T>(string[] arguments)
        where T : new()
    {
        var propertyNameMappings = GetPropertyNameMappings<T>();
        var (options, isValid, errormessage) = ExtractArguments<T>(arguments, propertyNameMappings);

        return new(options, isValid, errormessage);
    }

    public static Dictionary<string, (CommandLineOptionAttribute Attribute, PropertyInfo PropertyInfo)> GetAllOptions<T>()
    {
        var options = new Dictionary<string, (CommandLineOptionAttribute Attribute, PropertyInfo PropertyInfo)>();

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
                options.Add(longName, (attribute, property));
            }
        }

        return options;
    }

    private static Dictionary<string, PropertyInfo> GetPropertyNameMappings<T>()
    {
        var nameMappings = new Dictionary<string, PropertyInfo>();

        foreach (var (longName, (attribute, property)) in GetAllOptions<T>())
        {
            nameMappings.Add(longName, property);

            foreach (var shortName in attribute.ShortNames.Select(_ => _.ToLower()))
            {
                if (!string.IsNullOrWhiteSpace(shortName))
                {
                    nameMappings.Add(shortName, property);
                }
            }
        }

        return nameMappings;
    }

    private static (T Options, bool IsValid, string ErrorMessage) ExtractArguments<T>(string[] arguments, Dictionary<string, PropertyInfo> propertyNameMappings)
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
                    return Error<T>($"Expected parameter value for option {currentProperty.ArgName}");
                }

                var argName = (arg.Length > 1 && arg[1] == '-')
                    ? arg[2..]
                    : arg[1..];

                if (!propertyNameMappings.TryGetValue(argName, out var mappedProperty))
                {
                    return Error<T>($"Invalid option: {arg}");
                }

                if (mappedProperty.PropertyType == typeof(bool))
                {
                    if (!TrySetValue(mappedProperty, options, string.Empty, out var errorMessage))
                    {
                        return Error<T>(errorMessage);
                    }

                    currentProperty = (null, string.Empty);
                }
                else
                {
                    currentProperty = (mappedProperty, argName);
                }
            }
            else if (currentProperty.Info != null)
            {
                if (!TrySetValue(currentProperty.Info, options, arg, out var errorMessage))
                {
                    return Error<T>(errorMessage);
                }

                currentProperty = (null, string.Empty);
            }
            else
            {
                return Error<T>($"Invalid option: {arg}");
            }
        }

        return currentProperty.Info != null
            ? Error<T>($"Expected parameter value for option '{currentProperty.ArgName}'")
            : Success(options);
    }

    private static (T Options, bool IsValid, string ErrorMessage) Error<T>(string errorMessage)
        where T : new()
    {
        return (new(), false, errorMessage);
    }

    private static (T Options, bool IsValid, string ErrorMessage) Success<T>(T options)
        where T : new()
    {
        return (options, true, string.Empty);
    }

    private static bool TrySetValue<T>(PropertyInfo current, T options, string arg, out string errorMessage)
    {
        errorMessage = string.Empty;

        switch (current.PropertyType)
        {
            case var type when type == typeof(bool):
            {
                current.SetValue(options, true);
                return true;
            }

            case var type when type == typeof(int):
            {
                if (!int.TryParse(arg, out var value))
                {
                    errorMessage = $"Could not parse value as int: '{arg}'";
                    return false;
                }

                current.SetValue(options, value);
                return true;
            }

            case var type when type == typeof(long):
            {
                if (!long.TryParse(arg, out var value))
                {
                    errorMessage = $"Could not parse value as int: '{arg}'";
                    return false;
                }

                current.SetValue(options, value);
                return true;
            }

            case var type when type == typeof(string):
            {
                current.SetValue(options, arg);
                return true;
            }

            default:
            {
                errorMessage = $"Unsupported property value type: {current.PropertyType.Name}";
                return false;
            }
        }
    }
}
