using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.AggregationFunctions;

/// <summary>
/// Sums the values of a specified expression.
/// </summary>
public class SumFunction : IAggregationFunction
{
    private readonly string _expression;
    private readonly string? _alias;

    /// <summary>
    /// Creates a new instance of <see cref="SumFunction"/> for the specified expression.
    /// </summary>    
    public SumFunction(string expression, string? alias = null)
    {
        _expression = expression;
        _alias = alias;
    }

    /// <inheritdoc />
    public string ToKql(ParameterCollection parameters)
    {
        var result = _alias == null ? "" : $"{_alias} = ";
        result += $"sum({_expression})";
        return result;
    }
}
