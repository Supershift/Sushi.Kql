
namespace Sushi.Kql;

public interface IQueryBuilder
{
    string ToKqlString();
    Dictionary<string, string> GetParameters();
}