using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Data.Common;
using Sushi.Kql.Exceptions;
using Sushi.Kql.Mapping;

namespace Sushi.Kql;

/// <summary>
/// Executes KQL queries against a Kusto database.
/// </summary>
public class QueryClient
{
    private readonly ICslQueryProvider _client;

    /// <summary>
    /// Creates a new instance of <see cref="QueryClient"/>.
    /// </summary>
    /// <param name="client"></param>
    public QueryClient(ICslQueryProvider client)
    {
        _client = client;
    }

    /// <summary>
    /// Executes a KQL query and returns the results as an <see cref="IDataReader"/>. Called needs to dispose the returned reader after use.
    /// </summary>    
    public async Task<IDataReader> ExecuteQueryAsync(IQueryBuilder queryBuilder, string database, CancellationToken cancellationToken = default)
    {
        var kqlQuery = queryBuilder.ToKqlString();
        var parameters = queryBuilder.GetParameters();
        var properties = new ClientRequestProperties();
        if (parameters.Count > 0)
            properties.SetParameters(parameters);

        try
        {
            return await _client.ExecuteQueryAsync(database, kqlQuery, properties, cancellationToken);
        }
        catch(Exception ex)
        {
            throw new QueryExecutionException(kqlQuery, ex);
        }
    }

    /// <summary>
    /// Executes the query and returns all rows.
    /// </summary>    
    public async Task<QueryResult<T>> GetAllAsync<T>(QueryBuilder<T> queryBuilder, string database, CancellationToken cancellationToken = default)
    {
        using var reader = await ExecuteQueryAsync(queryBuilder, database, cancellationToken);
        var dataMap = queryBuilder.GetMap();
        var result = ResultMapper.MapToMultipleResults(reader, dataMap);
        return result;
    }
}
