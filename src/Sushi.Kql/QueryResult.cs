namespace Sushi.Kql;

/// <summary>
/// Represents the result to a query operation
/// </summary>
/// <typeparam name="T"></typeparam>
public record QueryResult<T>
{
    /// <summary>
    /// Rows returned by the query
    /// </summary>
    public required List<T> Data { get; init; }
}
