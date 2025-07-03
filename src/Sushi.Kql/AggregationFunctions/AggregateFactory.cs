using System.Linq.Expressions;
using Sushi.Kql.Mapping;

namespace Sushi.Kql.AggregationFunctions;
/// <summary>
/// Creates aggregation functions scoped to a type.
/// </summary>
public class AggregateFactory<T>
{
    private readonly DataMap<T> _map;

    /// <summary>
    /// Creates a new instance of the AggregateFactory for the specified data map.
    /// </summary>    
    public AggregateFactory(DataMap<T> map)
    {
        _map = map;
    }

    public IAggregationFunction ArgMax(Expression<Func<T, object?>> on, Expression<Func<T, object?>>[] columnsToReturn, string? alias = null)
    {
        var mapItem = _map.GetItem(on);
        var columns = new string[columnsToReturn.Length];
        for (int i = 0; i < columnsToReturn.Length; i++)
        {
            columns[i] = _map.GetItem(columnsToReturn[i]).Column;
        }
        return new ArgMaxFunction(mapItem.Column, columns, alias);
    }

    /// <summary>
    /// Calculates the average (arithmetic mean) of an expression across the group.
    /// </summary>
    /// <param name="on">Expression to calculate average on.</param>
    /// <param name="alias">Alias for the result</param>
    /// <param name="roundingPrecision">Number of digits to round result to. Default is 0.</param>    
    public IAggregationFunction Avg(Expression<Func<T, object?>> on, string? alias = null, int roundingPrecision = 0)
    {
        var mapItem = _map.GetItem(on);
        return new AvgFunction(mapItem.Column, alias ?? mapItem.Column, roundingPrecision);
    }

    public IAggregationFunction Count(string? alias = null, int? accuracy = null)
    {
        return new CountFunction(alias, accuracy);
    }

    public IAggregationFunction DCount(Expression<Func<T, object?>> on, string? alias, int? accuracy = null)
    {
        var mapItem = _map.GetItem(on);
        return new DCountFunction(mapItem.Column, alias, accuracy);
    }

    /// <summary>
    /// Arbitrarily chooses one record for each group in a summarize operator, and returns the value of one or more expressions over each such record.
    /// </summary>
    public IAggregationFunction TakeAny(Expression<Func<T, object?>> expression, string? alias = null)
    {
        var mapItem = _map.GetItem(expression);
        
        return new TakeAnyFunction([(mapItem.Column, alias)]);
    }

    /// <summary>
    /// Arbitrarily chooses one record for each group in a summarize operator, and returns the value of one or more expressions over each such record.
    /// </summary>
    public IAggregationFunction TakeAny(Expression<Func<T, object?>>[] expressions)
    {        
        var columns = new (string, string?)[expressions.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
            columns[i] = (_map.GetItem(expressions[i]).Column, null);
        }
        return new TakeAnyFunction(columns);
    }

    /// <summary>
    /// Arbitrarily chooses one record for each group in a summarize operator, and returns the value of one or more expressions over each such record.
    /// </summary>
    public IAggregationFunction TakeAny((Expression<Func<T, object?>> Expression, string? Alias)[] expressions)
    {
        var columns = new (string, string?)[expressions.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
            columns[i] = (_map.GetItem(expressions[i].Expression).Column, expressions[i].Alias);
        }
        return new TakeAnyFunction(columns);
    }

    /// <summary>
    /// Sums the values of the specified expression.
    /// </summary>    
    public IAggregationFunction Sum(Expression<Func<T, object?>> on, string? alias)
    {
        var mapItem = _map.GetItem(on);
        return new SumFunction(mapItem.Column, alias);
    }
}
