using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sushi.Kql.Mapping;
/// <summary>
/// Contains helper methods to convert between KQL and .NET types.
/// </summary>
internal static class Conversion
{
    internal static object? ConvertKqlReaderValueToTarget(object value, DataMapItem item)
    {
        object? result;
        // convert DBNull to null
        if (value == DBNull.Value)
        {
            result = null;
        }
        else if (item.DataType == KqlDataType.Boolean && value is sbyte)
        {
            // Booleans are read as SByte. SetMemberValue doesn't support this yet.
            result = Convert.ToBoolean(value);
        }
        else if (value is SqlDecimal valueDecimal)
        {
            // Decimals are read as SqlDecimal. SetMemberValue doesn't support this yet.
            result = valueDecimal.Value;
        }
        else if (value is JToken jtoken)
        {
            // handle array, objects and primtive values stored as dynamic data type
            result = jtoken.ToObject(item.MemberType);
        }
        else if (item.MemberType == typeof(DateTimeOffset) && value is DateTime valueDateTime)
        {
            // convert to datetimeoffset
            result = (DateTimeOffset)valueDateTime;
        }
        else if (item.MemberType == typeof(Guid) && value is string valueString && Guid.TryParse(valueString, out var guidValue))
        {
            // convert to Guid if the value is a string representation of a Guid, and target type is a guid
            result = guidValue;
        }
        else
        {
            result = value;
        }
        return result;
    }

    /// <summary>
    /// Determines the most suitable <see cref="KqlDataType"/> for a .NET <see cref="Type"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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
