using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.Mapping;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds the select part of a KQL query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnionBuilder<T>
    {
        private readonly DataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;
        private bool _isFirst;

        internal UnionBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
        {
            _map = map;
            _builder = builder;
            _parameters = parameters;
            _isFirst = true;            
        }

        /// <summary>
        /// Adds a column to the output containing the sourcename for each unioned query.
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public UnionBuilder<T> WithSourceName(string sourceName)
        {
            if (!string.IsNullOrWhiteSpace(sourceName))
            {
                _builder.Append($"withsource={sourceName} ");
            }
            return this;
        }

        /// <summary>
        /// Adds a statement with a table result to the union.
        /// </summary>        
        public UnionBuilder<T> AddUnion(string tableName, Action<QueryBuilder<T>> queryBuilder)
        {
            // create new querybuilder for the union, passing the existing parametercollection to keep those parameters unique
            var qb = new QueryBuilder<T>(_map, new StringBuilder(), _parameters, tableName);

            // invoke querybuilder action to build the query
            queryBuilder(qb);

            // add statement to union
            return AddUnion(qb);
        }

        /// <summary>
        /// Adds a statement with a table result to the union.
        /// </summary>        
        public UnionBuilder<T> AddUnion(Action<QueryBuilder<T>> queryBuilder)
        {
            // use map<T>'s tablename
            string tableName = _map.TableName!;

            return AddUnion(tableName, queryBuilder);
        }

        /// <summary>
        /// Adds a statement with a table result to the union.
        /// </summary>        
        public UnionBuilder<T> AddUnion(IQueryBuilder queryBuilder)
        {
            if (_isFirst)
            {
                _isFirst = false;
            }
            else
            {
                _builder.Append(',');
            }

            _builder.Append('(');
            _builder.Append(queryBuilder.ToKqlString(false));
            _builder.Append(')');

            return this;
        }
    }
}
