using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.Mapping;

namespace Sushi.Kql;
/// <summary>
/// Provides instances of <see cref="QueryBuilder{T}"/>.
/// </summary>
public class QueryBuilderProvider
{
    private readonly DataMapProvider _dataMapProvider;

    /// <summary>
    /// Creates a new instance of <see cref="QueryBuilderProvider"/>.
    /// </summary>    
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

    /// <summary>
    /// Gets an instance of <see cref="QueryBuilder{T}"/> to execute against <paramref name="tableName"/>.
    /// </summary>
    /// <param name="tableName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public QueryBuilder<T> GetBuilder<T>(string tableName)
    {
        var map = _dataMapProvider.GetMapForType<T>();
        return new QueryBuilder<T>(map, tableName);
    }
}
