using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.Operators;

namespace Sushi.Kql.AggregationFunctions
{
    public interface IAggregationFunction
    {
        /// <summary>
        /// Generate KQL for the function
        /// </summary>        
        string ToKql(ParameterCollection parameters);
    }
}
