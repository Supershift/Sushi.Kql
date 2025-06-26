using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.Mapping;

namespace Sushi.Kql.AggregationFunctions;

/// <summary>
/// Builds Summarize KQL statements, used for grouping and aggregating data.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SummarizeBuilder<T>
{
    private readonly DataMap<T> _map;
    private readonly StringBuilder _builder;
    private readonly ParameterCollection _parameters;
    private bool isFirst = true;

    internal SummarizeBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
    {
        _map = map;
        _builder = builder;
        _parameters = parameters;        
        builder.Append("| summarize ");
    }

    /// <summary>
    /// Adds custom kql to the Summarize statement.
    /// </summary>    
    public SummarizeBuilder<T> Agg(string kql)
    {
        if (!isFirst)
        {
            _builder.Append(", ");            
        }
        else
        {
            isFirst = false;
        }

        _builder.Append(kql);        

        return this;
    }

    /// <summary>
    /// Adds the aggregations to summarize to the statement.
    /// </summary>    
    public SummarizeBuilder<T> Agg(params IAggregationFunction[] on)
    {
        for (int i = 0; i < on.Length; i++)
        {   
            Agg(on[i].ToKql(_parameters));
        }
        return this;
    }

    /// <summary>
    /// Adds an aggregation to summarize to the statement.
    /// </summary>
    public SummarizeBuilder<T> Agg(Func<AggregateFactory<T>, IAggregationFunction> on)
    {
        return Agg(on(new AggregateFactory<T>(_map)));
    }

    /// <summary>
    /// Adds the aggregations to summarize to the statement.
    /// </summary>
    public SummarizeBuilder<T> Agg(Func<AggregateFactory<T>, IAggregationFunction[]> on)
    {
        return Agg(on(new AggregateFactory<T>(_map)));
    }

    /// <summary>
    /// Adds the "by" clause to the Summarize statement using a single column.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SummarizeBuilder<T> By(Expression<Func<T, object?>> expression, string? alias = null)
    {
        var byBuilder = new SummarizeByBuilder<T>(_map, _builder, _parameters);
        byBuilder.Term(expression, alias);
        return this;
    }

    /// <summary>
    /// Adds the "by" clause to the Summarize statement using a builder function.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public SummarizeBuilder<T> By(Action<SummarizeByBuilder<T>> builder)
    {
        builder(new SummarizeByBuilder<T>(_map, _builder, _parameters));
        return this;
    }
}
