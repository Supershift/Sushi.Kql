using System.Globalization;

namespace Sushi.Kql;

/// <summary>
///
/// </summary>
public class Parameter(KqlDataType kqlType, string parameterName, object? value)
{
    public string Type { get; init; } = kqlType.ToString().ToLower();
    public string Name { get; init; } = parameterName;
    public string Value { get; init; } = GetParameterValue(kqlType, value);

    private static string GetParameterValue(KqlDataType kqlType, object? value)
    {
        switch (kqlType)
        {
            case KqlDataType.Guid:
                if (value is Guid guidValue)
                {
                    return guidValue.ToString();
                }
                else if (value == null)
                {
                    return "guid(null)";
                }
                throw new ArgumentException($"Expected a GUID for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.String:
                return value?.ToString() ?? "";
            case KqlDataType.DateTime:
                if (value is DateTime dateTimeValue)
                {
                    return dateTimeValue.ToString("o", CultureInfo.InvariantCulture);
                }
                else if (value == null)
                {
                    return "datetime(null)";
                }
                throw new ArgumentException($"Expected a DateTime for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.TimeSpan:
                if (value is string)
                {
                    return value.ToString() ?? "";
                }
                throw new ArgumentException($"Expected a string for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.Int:
                if (value == null)
                {
                    return "int(null)";
                }
                else if (value is int intValue)
                {
                    return intValue.ToString();
                }
                else if (value.GetType().IsEnum)
                {
                    return ((int)value).ToString();
                }
                throw new ArgumentException($"Expected an int for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.Long:
                if (value == null)
                {
                    return "long(null)";
                }
                else if (value is long longValue)
                {
                    return longValue.ToString();
                }
                else if (value.GetType().IsEnum)
                {
                    return ((long)value).ToString();
                }
                throw new ArgumentException($"Expected a long for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.Real:
                if (value is float floatValue)
                {
                    return floatValue.ToString();
                }
                else if (value == null)
                {
                    return "real(null)";
                }
                throw new ArgumentException($"Expected an int for {kqlType}, but received {value.GetType()}.");
            case KqlDataType.Boolean:
                if (value is bool boolValue)
                {
                    return boolValue ? "true" : "false";
                }
                else if (value == null)
                {
                    return "bool(null)";
                }
                throw new ArgumentException(
                    $"Expected a bool for {kqlType}, but received {value.GetType()}.",
                    nameof(kqlType)
                );
            default:
                throw new NotSupportedException($"Type {kqlType} is not supported.");
        }
    }

}
