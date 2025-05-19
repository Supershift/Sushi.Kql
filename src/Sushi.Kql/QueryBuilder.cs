using System.Data;
using System.Text;
using Sushi.Kql.AggregationFunctions;

namespace Sushi.Kql;

/// <summary>
/// Builds a KQL query with a fluent API.
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueryBuilder<T>
{
    private readonly DataMap<T> _map;
    private readonly StringBuilder _builder;
    private readonly ParameterCollection _parameters;

    /// <summary>
    /// Creates a new instance of <see cref="QueryBuilder{T}"/>.
    /// </summary>        
    public QueryBuilder(DataMap<T> map)
    {
        _map = map;
        _builder = new StringBuilder(map.TableName);
        _parameters = new ParameterCollection();
    }

    /// <summary>
    /// Filters data from the table.
    /// </summary>
    /// <returns></returns>
    public WhereBuilder<T> Where()
    {
        _builder.AppendLine();
        return new WhereBuilder<T>(_map, _builder, _parameters);
    }

    /// <summary>
    /// Selects data from the table.
    /// </summary>
    /// <returns></returns>
    public SelectBuilder<T> Select()
    {
        _builder.AppendLine();
        return new SelectBuilder<T>(_map, _builder, _parameters);
    }

    /// <summary>
    /// Groups and aggregates data.
    /// </summary>
    /// <returns></returns>
    public SummarizeBuilder<T> Summarize()
    {
        _builder.AppendLine();
        return new SummarizeBuilder<T>(_map, _builder, _parameters);
    }

    /// <summary>
    /// Limits the results to a specified number of rows.
    /// </summary>        
    /// <returns></returns>
    public void Top(int numberOfRows, string expression)
    {
        _builder.AppendLine();
        _builder.Append($"| top({numberOfRows}) by {expression}");
    }

    /// <summary>
    /// Returns a KQL string representation of the query.
    /// </summary>
    /// <returns></returns>
    public string Build()
    {
        // add parameters
        var parameters = _parameters.GetParameters();
        if (parameters.Count > 0)
        {
            var stringified = string.Join(", ", parameters.Select(x => $"{x.Name}:{x.Type}"));
            _builder.Insert(0, $"declare query_parameters({stringified});\n");
        }
        return _builder.ToString();
    }

    /// <summary>
    /// Gets the parameters used in the query.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetParameters()
    {
        return _parameters.GetParameters().ToDictionary(x => x.Name, x => x.Value);
    }
}
