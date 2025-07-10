namespace Sushi.Kql;

/// <summary>
/// Defines an interface to build Kusto Query Language (KQL) queries.
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    /// Returns a KQL string representation of the query.
    /// </summary>
    /// <returns></returns>
    /// <param name="declareParameters">If true, 'declare query_parameters' is added to beginning of query.</param>
    string ToKqlString(bool declareParameters = true);

    /// <summary>
    /// Gets the parameters used in the query.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, string> GetParameters();
}
