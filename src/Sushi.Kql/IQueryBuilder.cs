
namespace Sushi.Kql;

public interface IQueryBuilder
{
    /// <summary>
    /// Returns a KQL string representation of the query.
    /// </summary>
    /// <returns></returns>
    /// <param name="declareParameters">If true, 'declare query_parameters' is added to beginning of query.</param>
    string ToKqlString(bool declareParameters = true);
    Dictionary<string, string> GetParameters();
}
