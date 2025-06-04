using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.AggregationFunctions;
using Sushi.Kql.Mapping;

namespace Sushi.Kql.TimeSeries;
/// <summary>
/// Builds a 'make-series' operator, used to create series of specified aggregated values along a specified axis.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MakeSeriesBuilder<T>
{
    private readonly DataMap<T> _map;
    private readonly StringBuilder _builder;
    private readonly ParameterCollection _parameters;

    internal MakeSeriesBuilder(MakeSeriesKind kind, DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
    {
        _map = map;
        _builder = builder;
        _parameters = parameters;
        builder.Append("| make-series ");
        if (kind == MakeSeriesKind.NonEmpty)
            builder.Append(" kind=nonempty ");
    }

    /// <summary>
    /// Adds a single aggregate to the make-series statement.
    /// </summary>
    /// <param name="on"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> Agg(Func<AggregateFactory<T>, IAggregationFunction> on)
    {
        var aggregate = on(new AggregateFactory<T>(_map));
        _builder.Append(aggregate.ToKql(_parameters));
        return this;
    }

    /// <summary>
    /// Adds multiple aggregates to the make-series statement.
    /// </summary>
    /// <param name="on"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> Agg(Func<AggregateFactory<T>, IAggregationFunction[]> on)
    {
        var aggregates = on(new AggregateFactory<T>(_map));
        for (int i = 0; i < aggregates.Length; i++)
        {
            if (i > 0)
                _builder.Append(", ");
            var aggregate = aggregates[i];
            _builder.Append(aggregate.ToKql(_parameters));
        }        
        return this;
    }

    /// <summary>
    /// Adds the 'on' clause to the make-series statement.
    /// </summary>
    /// <param name="on"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> On(Expression<Func<T, object?>> on)
    {
        var dataMapItem = _map.GetItem(on);
        _builder.Append(" on ").Append(dataMapItem.Column);
        return this;
    }

    /// <summary>
    /// Adds the 'from' clause to the make-series statement.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> From<TParam>(TParam from)
    {
        var parameter = _parameters.Add(from);
        _builder.Append(" from ").Append(parameter);
        return this;
    }

    /// <summary>
    /// Adds the 'to' clause to the make-series statement.
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> To<TParam>(TParam to)
    {
        var parameter = _parameters.Add(to);
        _builder.Append(" to ").Append(parameter);
        return this;
    }

    /// <summary>
    /// Adds the 'step' clause to the make-series statement.
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> Step(string step)
    {
        _builder.Append(" step ").Append(step);
        return this;
    }

    /// <summary>
    /// Adds the 'by' clause to the make-series statement. Note: don't use in combination with <see cref="MakeSeriesKind.NonEmpty"/>.
    /// </summary>    
    public MakeSeriesBuilder<T> By(Expression<Func<T, object?>> by, string? alias)
    {
        var dataMapItem = _map.GetItem(by);
        _builder.Append(" by ");
        if(!string.IsNullOrWhiteSpace(alias))
        {
            _builder.Append(alias).Append(" = ");
        }
        _builder.Append(dataMapItem.Column);
        return this;
    }

    /// <summary>
    /// Adds the "by" clause to the make-series statement using a builder function.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public MakeSeriesBuilder<T> By(Action<SummarizeByBuilder<T>> builder)
    {
        builder(new SummarizeByBuilder<T>(_map, _builder, _parameters));
        return this;
    }
}
