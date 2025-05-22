using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.AggregationFunctions
{
    public class ArgMaxFunction : IAggregationFunction
    {
        public ArgMaxFunction(string columnToMaximize, string[] columnsToReturn, string? alias)
        {
            ColumnToMaximize = columnToMaximize;
            ColumnsToReturn = columnsToReturn;
            Alias = alias;
        }

        public string ColumnToMaximize { get; }
        public string[] ColumnsToReturn { get; }
        public string? Alias { get; }

        public string ToKql(ParameterCollection parameters)
        {
            var result = Alias == null ? "" : $"{Alias} = ";
            result += $"arg_max({ColumnToMaximize}, {string.Join(',', ColumnsToReturn)})";
            return result;
        }
    }
}
