using System.Linq.Expressions;

namespace Sushi.Kql.AggregationFunctions;
public class AggregateFactory<T>
{
    private readonly DataMap<T> _map;

    public AggregateFactory(DataMap<T> map)
    {
        _map = map;
    }

    public IAggregationFunction DCount(Expression<Func<T, object?>> on, string? alias, int? accuracy = null)
    {
        var mapItem = _map.GetItem(on);
        return new DCountFunction(mapItem.Column, alias, accuracy);
    }

    public IAggregationFunction Count(string? alias = null, int? accuracy = null)
    {
        return new CountFunction(alias, accuracy);
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
}
