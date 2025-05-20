using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Sushi.Kql.Operators;

namespace Sushi.Kql;

/// <summary>
/// Builds a 'make-series' operator, used to create series of specified aggregated values along a specified axis.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MakeSeriesBuilder<T>
{
    private readonly DataMap<T> _map;
    private object? _start;
    private object? _stop;
    private string? _on;
    private IAggregationFunction? _aggregate;
    private string? _step;


    public MakeSeriesBuilder(DataMap<T> map)
    {
        _map = map;
    }

    public MakeSeriesBuilder<T> Count()
    {
        _aggregate = new CountFunction("num");
        return this;
    }

    public MakeSeriesBuilder<T> DistinctCount(Expression<Func<T, object?>> expression, int? accuracy = null)
    {
        var dataProperty = _map.GetItem(expression);
        _aggregate = new DistinctCountFunction(dataProperty.Column, "num", accuracy);
        return this;
    }

    public MakeSeriesBuilder<T> On(Expression<Func<T, object?>> on)
    {
        var dataProperty = _map.GetItem(on);
        _on = dataProperty.Column;
        return this;
    }

    public MakeSeriesBuilder<T> From(object from)
    {
        _start = from;
        return this;
    }

    public MakeSeriesBuilder<T> To(object to)
    {
        _stop = to;
        return this;
    }

    public MakeSeriesBuilder<T> Step(string step)
    {
        _step = step;
        return this;
    }

    public MakeSeriesOperator Build()
    {
        ThrowIfNull(_on);
        ThrowIfNull(_start);
        ThrowIfNull(_stop);
        ThrowIfNull(_step);
        ThrowIfNull(_aggregate);
        var result = new MakeSeriesOperator(_on, _aggregate, _start, _stop, _step);
        return result;
    }

    private static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string fieldName = "field")
    {
        if (value == null)
            throw new InvalidOperationException($"Cannot build query, set {fieldName} before calling build");
    }
}
