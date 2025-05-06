using System.Data;
using System.Linq.Expressions;
using Kusto.Data.Common;
using Kusto.Ingest;

namespace Sushi.Kql;

public class DataMapItem
{
    public string[] Path { get; set; } = [];
    public string Column { get; set; } = string.Empty;
    public KqlDataType DataType { get; set; } = KqlDataType.String;
}

/// <summary>
/// Represents the mapping between ADX table and code objects.
/// </summary>
public abstract class DataMap<T>
{
    public abstract string TableName { get; }

    private readonly Dictionary<string, DataMapItem> _columnMapping = [];

    public IngestionMapping IngestionMapping => new() { IngestionMappings = InternalMappings };
    private List<ColumnMapping> InternalMappings { get; set; } = [];

    protected void Map(Expression<Func<T, object?>> expression, string? columnName = null)
    {
        var memberTree = ReflectionHelper.GetMemberTree(expression);
        var path = memberTree.Select(x => x.Name).ToArray();
        // Generate default column name if none is provided
        if (string.IsNullOrWhiteSpace(columnName))
        {
            // create default column name from property path, Image.Url becomes Image_Url
            columnName = string.Join("_", path);
        }

        // add column mapping
        var propertyKey = GetPropertyKey(path);
        var memberType = ReflectionHelper.GetMemberType(memberTree.Pop());
        _columnMapping[propertyKey] = new DataMapItem()
        {
            Column = columnName,
            Path = path,
            DataType = Utility.GetKqlDataType(memberType),
        };

        // add ingestion mapping
        InternalMappings.Add(
            new ColumnMapping
            {
                ColumnName = columnName,
                Properties = new Dictionary<string, string> { ["Path"] = BuildIngestionPath(path) },
            }
        );
    }

    public DataMapItem GetDataProperty(Expression<Func<T, object?>> expression)
    {
        var memberTree = ReflectionHelper.GetMemberTree(expression);
        var path = memberTree.Select(x => x.Name).ToArray();
        var propertyKey = GetPropertyKey(path);
        if (_columnMapping.TryGetValue(propertyKey, out var item))
        {
            return item;
        }
        throw new ArgumentException($"No mapping found for property {propertyKey}");
    }

    /// <summary>
    /// Get the data property for the given member expression.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if no mapping could be found for provided member expression.</exception>
    public DataMapItem GetDataProperty(MemberExpression memberExpression)
    {
        var memberTree = ReflectionHelper.GetMemberTree(memberExpression);
        var path = memberTree.Select(x => x.Name).ToArray();
        var propertyKey = GetPropertyKey(path);
        if (_columnMapping.TryGetValue(propertyKey, out var item))
        {
            return item;
        }
        throw new ArgumentException($"No mapping found for property {propertyKey}");
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

