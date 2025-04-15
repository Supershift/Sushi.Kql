using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json.Linq;
using Sushi.MicroORM;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds a WHERE part of a KQL query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhereBuilder<T>
    {
        private readonly KustoDataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;
        private bool _isFirst = true;

        internal WhereBuilder(KustoDataMap<T> map, StringBuilder builder, ParameterCollection parameters)
        {
            _map = map;
            _builder = builder;
            _parameters = parameters;
            builder.Append("| where ");
        }

        /// <summary>
        /// Adds an equality check to the query, e.g. where column == value.
        /// </summary>        
        public WhereBuilder<T> Equals(Expression<Func<T, object?>> mappingExpression, object? value)
        {
            return Add(mappingExpression, value, ComparisonOperator.Equals);
        }

        /// <summary>
        /// Adds a between predicate to the query, e.g. where column between (from..to).
        /// </summary>
        /// <param name="mappingExpression"></param>
        /// <param name="from">Inclusive lower bound</param>
        /// <param name="to">Exclusive upper bound</param>
        /// <returns></returns>
        public WhereBuilder<T> Between(Expression<Func<T, object?>> mappingExpression, object? from, object? to)
        {
            var dataProperty = _map.GetDataProperty(mappingExpression);
            var fromParameter = _parameters.Add(dataProperty.SqlType, from);
            var toParameter = _parameters.Add(dataProperty.SqlType, to);
            return AddKql($"{dataProperty.Column} between ({fromParameter}..{toParameter})");
        }

        /// <summary>
        /// Adds a predicate to the query, based on the specified comparison operator.
        /// </summary>        
        public WhereBuilder<T> Add(Expression<Func<T, object?>> mappingExpression, object? value, ComparisonOperator comparisonOperator)
        {
            var dataproperty = _map.GetDataProperty(mappingExpression);

            return Add(dataproperty.Column, dataproperty.SqlType, value, comparisonOperator);
        }

        /// <summary>
        /// Adds custom KQL to the query.
        /// </summary>
        /// <param name="customKql"></param>
        /// <returns></returns>
        public WhereBuilder<T> AddKql(string customKql)
        {
            if (_isFirst)
                _isFirst = false;
            else
                _builder.Append(" and ");

            _builder.Append(customKql);
            return this;
        }

        private WhereBuilder<T> Add(string column, SqlDbType sqlType, object? value, ComparisonOperator comparisonOperator)
        {
            if (comparisonOperator == ComparisonOperator.In)
            {
                return AddWhereIn(column, sqlType, value);
            }
            else
            {
                string parameterName = _parameters.Add(sqlType, value);
                string operatorSymbol = comparisonOperator switch
                {
                    ComparisonOperator.Equals => "==",
                    ComparisonOperator.GreaterThanOrEquals => ">=",
                    ComparisonOperator.GreaterThan => ">",
                    ComparisonOperator.LessThanOrEquals => "<=",
                    ComparisonOperator.LessThan => "<",
                    _ => throw new ArgumentException($"{comparisonOperator} not yet supported", nameof(comparisonOperator)),
                };
                return AddKql($"{column} {operatorSymbol} {parameterName}");
            }
        }

        private WhereBuilder<T> AddWhereIn(string column, SqlDbType sqlType, object? value)
        {
            if (value is not IEnumerable items)
            {
                throw new ArgumentException(
                    $"Cannot build where clause. When using {nameof(ComparisonOperator.In)}, supply an IEnumerable as value."
                );
            }

            var whereInParameters = new List<string>();
            foreach (var item in items)
            {
                whereInParameters.Add(_parameters.Add(sqlType, item));
            }
            if (whereInParameters.Count > 0)
            {
                var whereInValueString = string.Join(",", whereInParameters);
                return AddKql($"{column} in ({whereInValueString})");
            }
            else
            {
                // make sure to return no results if an empty where in is used.
                return AddKql($"1 == 0");
            }
        }
    }
}
