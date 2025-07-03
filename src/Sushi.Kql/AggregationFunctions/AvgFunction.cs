namespace Sushi.Kql.AggregationFunctions;

/// <summary>
/// Calculates the average (arithmetic mean) of an expression across the group.
/// </summary>
public class AvgFunction : IAggregationFunction
{
    private readonly string _alias;
    private readonly int _roundingPrecision;
    private readonly string _column;

    /// <summary>
    /// Creates a new instance of the AvgFunction.
    /// </summary>
    /// <param name="expression">Expression to calculate average on.</param>
    /// <param name="alias">Alias for the result</param>
    /// <param name="roundingPrecision">Number of digits to round result to. Default is 0.</param>
    public AvgFunction(string expression, string alias, int roundingPrecision = 0)
    {
        _alias = alias;
        _roundingPrecision = roundingPrecision;
        _column = expression;
    }

    /// <inheritdoc/>    
    public string ToKql(ParameterCollection parameters)
    {
        return $"{_alias} = round(avg({_column}),{_roundingPrecision})";
    }
}
