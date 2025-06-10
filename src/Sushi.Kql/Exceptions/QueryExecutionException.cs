using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.Exceptions;
/// <summary>
/// Represents an error during query execution.
/// </summary>
public class QueryExecutionException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="QueryExecutionException"/> with a message indicating the query that failed.
    /// </summary>
    /// <param name="kqlQuery">Query that caused the exception to be thrown.</param>
    /// <param name="innerException"></param>
    public QueryExecutionException(string kqlQuery, Exception innerException) : base($"Error while executing query:{Environment.NewLine}{kqlQuery}", innerException)
    {
        
    }
}
