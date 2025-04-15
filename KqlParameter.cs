using System.Data;
using System.Globalization;

namespace Sushi.Kql;

/// <summary>
///
/// </summary>
public class KqlParameter
{
    private static class KqlType
    {
        public const string Guid = "guid";
        public const string String = "string";
        public const string DateTime = "datetime";
        public const string Int = "int";
        public const string Float = "real";
        public const string Boolean = "boolean";
    }

    public KqlParameter(SqlDbType sqlType, string parameterName, object? value)
    {
        Name = parameterName;

        switch (sqlType)
        {
            case SqlDbType.UniqueIdentifier:
                Type = KqlType.Guid;
                if (value is Guid guidValue)
                {
                    Value = guidValue.ToString();
                    return;
                }
                else if (value == null)
                {
                    Value = "guid(null)";
                    return;
                }
                throw new ArgumentException($"Expected a GUID for {sqlType}, but received {value.GetType()}.");

            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.VarChar:
            case SqlDbType.NVarChar:
                Value = value?.ToString() ?? "";
                Type = KqlType.String;
                return;
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
            case SqlDbType.DateTimeOffset:
                Type = KqlType.DateTime;
                if (value is DateTime dateTimeValue)
                {
                    Value = dateTimeValue.ToString("o", CultureInfo.InvariantCulture);
                    return;
                }
                else if (value == null)
                {
                    Value = "datetime(null)";
                    return;
                }
                throw new ArgumentException($"Expected a DateTime for {sqlType}, but received {value.GetType()}.");
            case SqlDbType.Int:
                Type = KqlType.Int;
                if (value is int intValue)
                {
                    Value = intValue.ToString();
                    return;
                }
                else if (value == null)
                {
                    Value = "int(null)";
                    return;
                }
                throw new ArgumentException($"Expected an int for {sqlType}, but received {value.GetType()}.");
            case SqlDbType.Float:
                Type = KqlType.Float;
                if (value is float floatValue)
                {
                    Value = floatValue.ToString();
                    return;
                }
                else if (value == null)
                {
                    Value = "real(null)";
                    return;
                }
                throw new ArgumentException($"Expected an int for {sqlType}, but received {value.GetType()}.");
            case SqlDbType.Bit:
                Type = KqlType.Boolean;
                if (value is bool boolValue)
                {
                    Value = boolValue ? "true" : "false";
                    return;
                }
                else if (value == null)
                {
                    Value = "bool(null)";
                    return;
                }
                throw new ArgumentException($"Expected a bool for {sqlType}, but received {value.GetType()}.", nameof(sqlType));
            default:
                throw new NotSupportedException($"Type {sqlType} is not supported.");
        }
    }

    public string Type { get; init; }
    public string Name { get; init; }
    public string Value { get; init; }
}
