namespace Sushi.Kql.AggregationFunctions;

public class DCountFunction : IAggregationFunction
{
    private readonly string? _alias;

    public DCountFunction(string column, string? alias = null, int? accuracy = null)
    {
        Column = column;
        _alias = alias;
        Accuracy = accuracy;
    }

    public string Column { get; }
    public int? Accuracy { get; }

    public string ToKql(ParameterCollection parameters)
    {
        var result = _alias == null ? "" : $"{_alias} = ";
        result += $"dcount({Column}";
        if (Accuracy != null)
            result += $", {Accuracy}";
        result += ")";
        return result;

    }
}
