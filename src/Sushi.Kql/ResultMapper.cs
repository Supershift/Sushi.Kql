using System.Data;
using System.Data.SqlTypes;
using Newtonsoft.Json.Linq;

namespace Sushi.Kql;
public static class ResultMapper
{
    public static List<T> MapToMultipleResults<T>(IDataReader reader, DataMap<T> map)
    {
        var result = new List<T>();
        while (reader.Read())
        {
            T? instance;
            try
            {
                instance = (T)Activator.CreateInstance(typeof(T), true)!;
            }
            catch (Exception ex)
            {
                throw new MissingMethodException($"Cannot create instance of {typeof(T).Name}. Please add a parameterless constructor.", ex);
            }
            MapRowToObject(reader, map, instance);
            result.Add(instance);
        }

        return result;
    }

    internal static void MapRowToObject<T>(IDataReader reader, DataMap<T> map, T instance)
    {
        if (EqualityComparer<T>.Default.Equals(instance, default))
        {
            throw new ArgumentNullException(nameof(instance));
        }

        // for each mapped member on the instance, go through the result set and find a column with the expected name
        foreach (var item in map.Items.Values)
        {
            // which name is expected in the result set by the mapped item
            string mappedName = item.Column;

            // find a column matching the mapped name
            for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                // get the name of the column as returned by the database
                var columnName = reader.GetName(columnIndex);

                if (mappedName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var value = reader.GetValue(columnIndex);

                    var memberType = item.DataType;

                    // convert DBNull to null
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                    else if (memberType == KqlDataType.Boolean && value is sbyte)
                    {
                        // Booleans are read as SByte. SetMemberValue doesn't support this yet.
                        value = Convert.ToBoolean(value);
                    }
                    else if (value is SqlDecimal valueDecimal)
                    {
                        // Decimals are read as SqlDecimal. SetMemberValue doesn't support this yet.
                        value = valueDecimal.Value;
                    }
                    else if (value is string valueString && string.IsNullOrEmpty(valueString))
                    {
                        // kusto doesnt support null strings, so we convert them to null for now
                        value = null;
                    }
                    else if (value is JToken jtoken)
                    {
                        // handle array, objects and primtive values stored as dynamic data type
                        value = jtoken.ToObject(item.MemberType);
                    }

                    ReflectionHelper.SetNestedProperty(instance!, item.Path, value);
                    break;
                }
            }
        }
    }
}
