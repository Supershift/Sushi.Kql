using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.Operators
{
    public class ArgMaxFunction : IAggregationFunction
    {
        public ArgMaxFunction(string columnToMaximize, string[] columnsToReturn)
        {
            ColumnToMaximize = columnToMaximize;
            ColumnsToReturn = columnsToReturn;
        }

        public string ColumnToMaximize { get; }
        public string[] ColumnsToReturn { get; }

        public string ToKql(ParameterCollection parameters)
        {
            return $"arg_max({ColumnToMaximize}, {string.Join(',', ColumnsToReturn)})";
        }
    }
}
