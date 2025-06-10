using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.Mapping;

namespace Sushi.Kql.AggregationFunctions;

/// <summary>
/// Used to build the "by" part of a Summarize KQL statement.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SummarizeByBuilder<T>
{
    private readonly DataMap<T> _map;
    private readonly StringBuilder _builder;
    private readonly ParameterCollection _parameters;
    private int _count;

    /// <summary>
    /// Creates a new instance of <see cref="SummarizeByBuilder{T}"/>.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="builder"></param>
    /// <param name="parameters"></param>
    public SummarizeByBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
    {
        _map = map;
        _builder = builder;
        _parameters = parameters;
        _count = 0;
        _builder.Append(" by ");
    }

    /// <summary>
    /// Adds a grouping expression on a term to the by clause, e.g. by 'ColumnName'
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SummarizeByBuilder<T> Term(Expression<Func<T, object?>> expression, string? alias = null)
    {   
        var dataProperty = _map.GetItem(expression);
        AddItem(dataProperty.Column, alias);        
        return this;
    }

    /// <summary>
    /// Adds a grouping expression on a bin to the by clause, e.g. by bin('DateTimeColumnName', 1d).
    /// </summary>    
    public SummarizeByBuilder<T> Bin(Expression<Func<T, object?>> expression, string roundTo, string? alias = null)
    {   
        var dataProperty = _map.GetItem(expression);
        string roundToParameter = _parameters.Add(KqlDataType.TimeSpan, roundTo);
        AddItem($"bin({dataProperty.Column}, {roundToParameter})", alias);
        return this;
    }

    private void AddItem(string groupExpression, string? alias)
    {
        _count++;
        if (_count > 1)
        {
            _builder.Append(", ");
        }
        if (!string.IsNullOrWhiteSpace(alias))
        {
            _builder.Append(alias).Append(" = ");
        }
        _builder.Append(groupExpression);
    }
}
