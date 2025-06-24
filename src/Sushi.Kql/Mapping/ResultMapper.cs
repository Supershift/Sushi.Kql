using System.Data;
using System.Data.Common;

namespace Sushi.Kql.Mapping;

/// <summary>
/// Provides methods to read results from a <see cref="DbDataReader"/> to objects, based on <see cref="DataMap"/>.
/// </summary>
public static class ResultMapper
{
    /// <summary>
    /// Maps all rows found in the first resultset of <paramref name="reader"/> to a collection of objects of type <typeparamref name="T"/> using the provided <paramref name="map"/>.            
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

    /// <summary>
    /// Maps a single data record to an object of type <typeparamref name="T"/> using the provided <paramref name="map"/>.   
    /// </summary>
    public static T MapRecordToObject<T>(IDataRecord record, DataMap<T> map)
    {
        T instance = (T)Activator.CreateInstance(typeof(T), true)!;
        SetResultValuesToObject(record, map, instance);
        return instance;
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

                    var convertedValue = Conversion.ConvertKqlReaderValueToTarget(value, item);

                    ReflectionHelper.SetMemberValue(item.MemberInfoTree, convertedValue, instance);
                    break;
                }
            }
        }
    }


}
