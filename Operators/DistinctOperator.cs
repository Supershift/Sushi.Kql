using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.Operators
{
    /// <summary>
    /// Represents a 'distinct' operator, used to get all distinct values for a column.
    /// </summary>
    public class DistinctOperator : IKqlOperator
    {
        private readonly string _columnName;

        public DistinctOperator(string columnName)
        {
            _columnName = columnName;
        }

        public string ToKql(ParameterCollection parameters)
        {
            return $"distinct {_columnName}";
        }
    }
}
