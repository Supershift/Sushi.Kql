using System.Data;

namespace Sushi.Kql;
public interface IQueryClient
{
    /// <summary>
    /// Executes a KQL query and returns the results as an <see cref="IDataReader"/>. Called needs to dispose the returned reader after use.
    /// </summary>    
    Task<IDataReader> ExecuteQueryAsync(IQueryBuilder queryBuilder, string database, CancellationToken cancellationToken = default);
    /// <summary>
    /// Executes the query and returns all rows.
    /// </summary>    
    Task<QueryResult<T>> GetAllAsync<T>(QueryBuilder<T> queryBuilder, string database, CancellationToken cancellationToken = default);
}
