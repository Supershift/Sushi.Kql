using System.Linq.Expressions;

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

    internal DataMapItem GetDataProperty<T>(Expression<Func<T, object?>> mappingExpression) => throw new NotImplementedException();
}

