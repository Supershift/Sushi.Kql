using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.AggregationFunctions
{
    /// <summary>
    /// Arbitrarily chooses one record for each group in a summarize operator, and returns the value of one or more expressions over each such record.
    /// </summary>
    public class TakeAnyFunction : IAggregationFunction
    {
        /// <summary>
        /// Creates a new instance of <see cref="TakeAnyFunction"/> with the specified expressions.
        /// </summary>
        /// <param name="expressions"></param>
        public TakeAnyFunction((string Expression, string? Alias)[] expressions)
        {
            Expressions = expressions;
        }

        /// <summary>
        /// Gets the expressions to return.
        /// </summary>
        public (string Expression, string? Alias)[] Expressions { get; }

        /// <inheritdoc/>        
        public string ToKql(ParameterCollection parameters)
        {
            var expressions = Expressions.Select(e => e.Alias != null ? $"{e.Alias} = {e.Expression}" : e.Expression);
            string result = $"take_any({string.Join(',', expressions)})";
            return result;
        }
    }
}
