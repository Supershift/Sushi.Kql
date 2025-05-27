namespace Sushi.Kql;

public static class Utility
{
    public static KqlDataType GetKqlDataType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            type = underlyingType;
        }

        if (type.IsEnum)
        {
            type = type.GetEnumUnderlyingType();
        }

        if (type == typeof(int) || type == typeof(short) || type == typeof(byte))
        {
            return KqlDataType.Int;
        }
        else if (type == typeof(long))
        {
            return KqlDataType.Long;
        }
        else if (type == typeof(bool))
        {
            return KqlDataType.Boolean;
        }
        else if (type == typeof(Guid))
        {
            return KqlDataType.Guid;
        }
        else if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly))
        {
            return KqlDataType.DateTime;
        }
        else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
        {
            return KqlDataType.Real;
        }
        else if (type == typeof(TimeSpan) || type == typeof(TimeOnly))
        {
            return KqlDataType.TimeSpan;
        }
        else if (type == typeof(byte[]))
        {
            return KqlDataType.Dynamic;
        }

        return KqlDataType.String;
    }

    /// <summary>
    /// Converts <paramref name="value"/> to an enumeration member if <paramref name="type"/> or its underlying <see cref="Type"/> is an <see cref="Enum"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? ConvertValueToEnum(object? value, Type type)
    {

        if (type.IsEnum && value != null)
        {
            value = Enum.ToObject(type, value);
        }

        return value;
    }
}
