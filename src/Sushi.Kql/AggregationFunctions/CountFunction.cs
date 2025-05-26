using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.AggregationFunctions;

/// <summary>
/// Counts all rows in the summarization group
/// </summary>
public class CountFunction : IAggregationFunction
{
    private readonly string? _alias;

    /// <summary>
    /// Creates a new instance of <see cref="CountFunction"/>.
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="accuracy"></param>
    public CountFunction(string? alias = null, int? accuracy = null)
    {   
        _alias = alias;
        Accuracy = accuracy;
    }
    
    public int? Accuracy { get; }

    public string ToKql(ParameterCollection parameters)
    {
        var result = _alias == null ? "" : $"{_alias} = ";
        result += "count()";
        return result;

    }
}
