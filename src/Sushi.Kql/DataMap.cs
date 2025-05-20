using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using Kusto.Data.Common;
using Kusto.Ingest;

namespace Sushi.Kql;

/// <summary>
/// Represents the mapping between ADX table and code objects.
/// </summary>
public abstract class DataMap<T>
{
    public string? TableName { get; private set;  }

    private readonly Dictionary<string, DataMapItem> _items = [];

    /// <summary>
    /// Gets all mapped items.
    /// </summary>
    public IReadOnlyDictionary<string, DataMapItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Sets the name of the table in ADX.
    /// </summary>
    /// <param name="tableName"></param>
    protected void Table(string tableName)
    {
        TableName = tableName;
    }

    /// <summary>
    /// Defines mapping between a property and a column in ADX.
    /// </summary>
    /// <param name="expression">Expression resolving to the property to map, e.g. x => x.MyRecord.MyField</param>
    /// <param name="columnName">Name of the column in ADX</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected DataMapItemSetter Map(Expression<Func<T, object?>> expression, string? columnName = null)
    {
        var memberTree = ReflectionHelper.GetMemberTree(expression);        
        var path = memberTree.Select(x => x.Name).ToArray();
        var propertyKey = GetPropertyKey(path);
        // Generate default column name if none is provided
        if (string.IsNullOrWhiteSpace(columnName))
        {
            // create default column name from property path, Image.Url becomes Image_Url
            columnName = string.Join("_", path);
        }

        // check if not already mapped
        if (Items.ContainsKey(propertyKey))
        {
            throw new ArgumentException($"Expression {propertyKey} is already mapped.");
        }

        var memberType = ReflectionHelper.GetMemberType(memberTree.Pop());
        var kqlType = Utility.GetKqlDataType(memberType);
        var ingestionMapping = new ColumnMapping
        {
            ColumnName = columnName,
            Properties = new Dictionary<string, string> { ["Path"] = BuildIngestionPath(path) },
        };
        var mapItem = new DataMapItem(path, columnName, kqlType, memberType, ingestionMapping);
        _items[propertyKey] = mapItem;
        return new DataMapItemSetter(mapItem);
    }

    /// <summary>
    /// Gets the mapping item for the given expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown if no mapping could be found for provided member expression.</exception>
    public DataMapItem GetItem(Expression<Func<T, object?>> expression)
    {
        var memberTree = ReflectionHelper.GetMemberTree(expression);
        var path = memberTree.Select(x => x.Name).ToArray();
        var propertyKey = GetPropertyKey(path);
        if (Items.TryGetValue(propertyKey, out var item))
        {
            return item;
        }
        throw new ArgumentException($"No mapping found for property {propertyKey}");
    }

    /// <summary>
    /// Gets the mapping item for the given member expression.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if no mapping could be found for provided member expression.</exception>
    public DataMapItem GetItem(MemberExpression memberExpression)
    {
        var memberTree = ReflectionHelper.GetMemberTree(memberExpression);
        var path = memberTree.Select(x => x.Name).ToArray();
        var propertyKey = GetPropertyKey(path);
        if (Items.TryGetValue(propertyKey, out var item))
        {
            return item;
        }
        throw new ArgumentException($"No mapping found for property {propertyKey}");
    }

    /// <summary>
    /// Gets the ingestion mapping for the mapped items.
    /// </summary>
    /// <returns></returns>
    public List<ColumnMapping> GetIngestionMapping()
    {
        return Items.Values.Where(x => !x.IsReadOnly)
            .Select(x => x.IngestionMapping)
            .ToList();
    }

    private static string BuildIngestionPath(string[] path)
    {
        return $"$.{string.Join(".", path)}";
    }

    private static string GetPropertyKey(string[] path)
    {
        return string.Join("_", path);
    }
}

