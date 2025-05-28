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

    internal SummarizeBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
    {
        _map = map;
        _builder = builder;
        _parameters = parameters;        
        builder.Append("| summarize ");
    }

    public SummarizeBuilder<T> On(params IAggregationFunction[] on)
    {
        for (int i = 0; i < on.Length; i++)
        {
            if (i > 0)
                _builder.Append(", ");
            _builder.Append(on[i].ToKql(_parameters));
        }
        return this;
    }

    public SummarizeBuilder<T> On(Func<AggregateFactory<T>, IAggregationFunction> on)
    {
        return On(on(new AggregateFactory<T>(_map)));
    }

    public SummarizeBuilder<T> On(Func<AggregateFactory<T>, IAggregationFunction[]> on)
    {
        return On(on(new AggregateFactory<T>(_map)));
    }

    public SummarizeBuilder<T> By(params Expression<Func<T, object?>>[] expressions)
    {
        if (expressions.Length > 0)
        {
            _builder.Append(" by ");
            for (int i = 0; i < expressions.Length; i++)
            {
                if (i > 0)
                    _builder.Append(", ");
                var dataProperty = _map.GetItem(expressions[i]);
                _builder.Append(dataProperty.Column);
            }
        }
        return this;
    }
}
