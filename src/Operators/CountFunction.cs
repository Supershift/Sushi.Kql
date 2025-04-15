using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.Operators
{
    public class CountFunction : IAggregationFunction
    {
        private readonly string? _alias;

        public CountFunction(string? alias)
        {
            _alias = alias;
        }

        public string ToKql(ParameterCollection parameters)
        {
            string prefix = _alias == null ? "" : $"{_alias} = ";            
            return $"{prefix}count()";            
        }
    }
}
