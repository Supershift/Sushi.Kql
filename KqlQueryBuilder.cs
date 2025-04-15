using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds a KQL query with a fluent API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KqlQueryBuilder<T>
    {
        private readonly KustoDataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;

        /// <summary>
        /// Creates a new instance of <see cref="KqlQueryBuilder{T}"/>.
        /// </summary>        
        public KqlQueryBuilder(KustoDataMap<T> map)
        {
            _map = map;
            _builder = new StringBuilder(map.TableName);
            _parameters = new ParameterCollection();
        }

        /// <summary>
        /// Filters data from the table.
        /// </summary>
        /// <returns></returns>
        public WhereBuilder<T> Where()
        {
            _builder.AppendLine();
            return new WhereBuilder<T>(_map, _builder, _parameters);
        }

        /// <summary>
        /// Selects data from the table.
        /// </summary>
        /// <returns></returns>
        public SelectBuilder<T> Select()
        {
            _builder.AppendLine();
            return new SelectBuilder<T>(_map, _builder, _parameters);
        }

        /// <summary>
        /// Groups and aggregates data.
        /// </summary>
        /// <returns></returns>
        public SummarizeBuilder<T> Summarize()
        {
            _builder.AppendLine();
            return new SummarizeBuilder<T>(_map, _builder, _parameters);
        }

        /// <summary>
        /// Limits the results to a specified number of rows.
        /// </summary>        
        /// <returns></returns>
        public void Top(int numberOfRows, string expression)
        {
            _builder.AppendLine();
            _builder.Append($"| top({numberOfRows}) by {expression}");
        }

        /// <summary>
        /// Returns a KQL string representation of the query.
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            // add parameters
            var stringified = string.Join(", ", _parameters.GetParameters().Select(x => $"{x.Name}:{x.Type}"));

            _builder.Insert(0, $"declare query_parameters({stringified});\n");

            return _builder.ToString();
        }

        /// <summary>
        /// Gets the parameters used in the query.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters()
        {
            return _parameters.GetParameters().ToDictionary(x => x.Name, x => x.Value);
        }
    }
}
