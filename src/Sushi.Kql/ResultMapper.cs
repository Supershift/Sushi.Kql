using System.Data;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Reflection;

namespace Sushi.Kql;

/// <summary>
/// Provides methods to read results from a <see cref="DbDataReader"/> to objects, based on <see cref="DataMap"/>.
/// </summary>
public static class ResultMapper
{
    /// <summary>
    /// Maps all rows found in the first resultset of <paramref name="reader"/> to a collectiobn of objects of type <typeparamref name="T"/> using the provided <paramref name="map"/>.            
    /// </summary>    
    public static QueryResult<T> MapToMultipleResults<T>(IDataReader reader, DataMap<T> map)
    {
        var result = new List<T>();
        //read all rows from the first resultset
        while (reader.Read())
        {
            T instance = (T)Activator.CreateInstance(typeof(T), true)!;
            SetResultValuesToObject(reader, map, instance);
            result.Add(instance);
        }

        return new QueryResult<T>() { Data = result };
    }

    internal static void SetResultValuesToObject<T, TResult>(IDataRecord reader, DataMap<T> map, TResult instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        // for each mapped member on the instance, go through the result set and find a column with the expected name
        for (int i = 0; i < map.Items.Count; i++)
        {
            var item = map.Items.Values.ElementAt(i);
            string mappedName = item.Column;            

            // find a column matching the mapped name
            for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                // get the name of the column as returned by the database
                var columnName = reader.GetName(columnIndex);

                if (mappedName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var value = reader.GetValue(columnIndex);

                    var convertedValue = ConvertReaderValueToTargetValue(value, item);

                    ReflectionHelper.SetMemberValue(item.MemberInfoTree, convertedValue, instance);
                    break;
                }
            }
        }
    }

    internal static object? ConvertReaderValueToTargetValue(object value, DataMapItem item)
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
}
