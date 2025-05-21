using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql;
/// <summary>
/// Provides instances of <see cref="QueryBuilder{T}"/>.
/// </summary>
public class QueryBuilderProvider
{
    private readonly DataMapProvider _dataMapProvider;

    public QueryBuilderProvider(DataMapProvider dataMapProvider)
    {
        _dataMapProvider = dataMapProvider;
    }

    /// <summary>
    /// Gets an instance <see cref="QueryBuilder{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public QueryBuilder<T> GetBuilder<T>()
    {
        var map = _dataMapProvider.GetMapForType<T>();
        return new QueryBuilder<T>(map);
    }
}
