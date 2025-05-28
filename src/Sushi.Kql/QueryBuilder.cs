using System.Data;
using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.AggregationFunctions;
using Sushi.Kql.Mapping;

namespace Sushi.Kql;

/// <summary>
/// Builds a KQL query with a fluent API.
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueryBuilder<T> : IQueryBuilder
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
    /// Gets the map used in the query builder.
    /// </summary>
    /// <returns></returns>
    public DataMap<T> GetMap()
    {
        return _map;
    }

    /// <summary>
    /// Adds a line of plain text KQL to the query.
    /// </summary>
    /// <param name="kql"></param>
    public void AddKql(string kql)
    {
        _builder.AppendLine();
        _builder.Append("| ").Append(kql);
    }

    /// <summary>
    /// Creates a new WHERE clause in the query. Chain predicates directly, e.g. Query.Where().Equals(x=>x.Column, value).Equals(x => x.OtherColumn, otherValue).
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
    public string ToKqlString()
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

    /// <summary>
    /// Limits output to a specified number of rows. Unless the data is sorted, the rows returned are not guaranteed to be the same each time the query is run.
    /// </summary>
    /// <param name="numberOfRows"></param>
    public void Limit(int numberOfRows)
    {
        _builder.AppendLine();
        _builder.Append($"| limit {numberOfRows}");
    }

    /// <summary>
    /// Takes N number of rows of rows. Unless the data is sorted, the rows returned are not guaranteed to be the same each time the query is run.
    /// </summary>
    /// <param name="numberOfRows"></param>
    public void Take(int numberOfRows)
    {
        _builder.AppendLine();
        _builder.Append($"| take {numberOfRows}");
    }
}
