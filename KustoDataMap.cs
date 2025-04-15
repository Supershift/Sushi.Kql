using System.Data;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using Kusto.Cloud.Platform.Data;
using Kusto.Data.Common;
using Kusto.Ingest;
using Sushi.MicroORM.Mapping;

namespace Sushi.Kql;

/// <summary>
/// Represents the mapping between ADX table and code objects.
/// </summary>
public abstract class KustoDataMap<T>
{
    /// <summary>
    /// Name of the ADX table
    /// </summary>
    public abstract string TableName { get; }

    public IngestionMapping IngestionMapping => new IngestionMapping() { IngestionMappings = InternalMappings };

    public DataMap<T> DataMap { get; set; } = new DataMap<T>();

    private List<ColumnMapping> InternalMappings { get; set; } = [];

    public DataMapItemSetter Map(Expression<Func<T, object?>> memberExpression)
    {
        // derive column name from member expression
        var memberTree = MicroORM.Supporting.ReflectionHelper.GetMemberTree(memberExpression);
        string columnName = $"{string.Join("_", memberTree.Select(x => x.Name))}";

        var result = DataMap.Map(memberExpression, columnName);

        string path = $"$.{string.Join(".", memberTree.Select(x => x.Name))}";

        InternalMappings.Add(
            new ColumnMapping
            {
                ColumnName = columnName,
                Properties = new Dictionary<string, string>() { { "Path", path } }
            }
        );

        return result;
    }

    public void Map(Expression<Func<T, object?>> memberExpression, string columnName)
    {
        DataMap.Map(memberExpression, columnName);

        var memberTree = MicroORM.Supporting.ReflectionHelper.GetMemberTree(memberExpression);
        var path = $"$.{string.Join(".", memberTree.Select(x => x.Name))}";
        InternalMappings.Add(
            new ColumnMapping
            {
                ColumnName = columnName,
                Properties = new Dictionary<string, string>() { { "Path", path } }
            }
        );
    }

    /// <summary>
    /// Get the data property for the given member expression.
    /// </summary>    
    /// <exception cref="ArgumentException">Thrown if no mapping could be found for provided expression.</exception>
    public DataMapItem GetDataProperty(Expression<Func<T, object?>> mappingExpression)
    {
        var members = MicroORM.Supporting.ReflectionHelper.GetMemberTree(mappingExpression);

        var dataProperty = DataMap.Items.FirstOrDefault(x => x.MemberInfoTree.SequenceEqual(members));
        if (dataProperty == null)
            throw new ArgumentException($"Could not find member [{string.Join(".", members.Select(x => x.Name))}] for type {typeof(T)}", nameof(mappingExpression));

        return dataProperty;
    }

    /// <summary>
    /// Maps the result set from the reader to the object.
    /// </summary>
    public T MapFromReader(IDataReader reader)
    {
        var instance = (T)Activator.CreateInstance(typeof(T), true)!;

        // for each mapped member on the instance, go through the result set and find a column with the expected name
        for (int i = 0; i < DataMap.Items.Count; i++)
        {
            var dataMapItem = DataMap.Items[i];

            // which name is expected in the result set by the mapped item
            string mappedName = dataMapItem.Column;
            if (!string.IsNullOrWhiteSpace(dataMapItem.Alias))
                mappedName = dataMapItem.Alias;

            // find a column matching the mapped name
            for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                // get the name of the column as returned by the database
                var columnName = reader.GetName(columnIndex);

                if (mappedName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var value = reader.GetValue(columnIndex);

                    var memberType = MicroORM.Supporting.ReflectionHelper.GetMemberType(dataMapItem.MemberInfoTree[dataMapItem.MemberInfoTree.Count - 1]);

                    // convert DBNull to null
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                    else if (memberType == typeof(bool) && value is sbyte)
                    {
                        // Booleans are read as SByte. SetMemberValue doesn't support this yet.
                        value = Convert.ToBoolean(value);
                    }
                    else if (value is SqlDecimal valueDecimal)
                    {
                        // Decimals are read as SqlDecimal. SetMemberValue doesn't support this yet.
                        value = valueDecimal.ToDecimal();
                    }
                    else if (value is string valueString && string.IsNullOrEmpty(valueString))
                    {
                        // kusto doesnt support null strings, so we convert them to null for now
                        value = null;
                    }

                    MicroORM.Supporting.ReflectionHelper.SetMemberValue(
                        dataMapItem.MemberInfoTree,
                        value,
                        instance!,
                        null,
                        null
                    );
                    break;
                }
            }
        }

        return instance!;
    }
}
