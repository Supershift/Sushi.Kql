using System.Linq.Expressions;

namespace Sushi.Kql.AggregationFunctions;
public class AggregateFactory<T>
{
    private readonly DataMap<T> _map;

    public AggregateFactory(DataMap<T> map)
    {
        _map = map;
    }

    public IAggregationFunction DCount(Expression<Func<T, object?>> on, string alias, int? accuracy = null)
    {
        var mapItem = _map.GetDataProperty(on);
        return new DCountFunction(mapItem.Column, alias, accuracy);
    }
}
