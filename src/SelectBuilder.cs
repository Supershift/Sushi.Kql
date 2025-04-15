using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json.Linq;
using Sushi.Kql.Operators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds the select part of a KQL query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectBuilder<T>
    {
        private readonly KustoDataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;
        private bool _isFirst;

        internal SelectBuilder(KustoDataMap<T> map, StringBuilder builder, ParameterCollection parameters)
        {
            _map = map;
            _builder = builder;
            _parameters = parameters;
            _isFirst = true;
            builder.Append("|  ");
        }

        /// <summary>
        /// Gets all distinct values for a column.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectBuilder<T> Distinct(Expression<Func<T, object?>> expression)
        {
            var dataProperty = _map.GetDataProperty(expression);
            var distinctOperator = new DistinctOperator(dataProperty.Column);
            return Add(distinctOperator);
        }

        /// <summary>
        /// Creates series of specified aggregated values along a specified axis.
        /// </summary>
        /// <param name="on"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public SelectBuilder<T> MakeSeries(Action<MakeSeriesBuilder<T>> buildAction)
        {
            var builder = new MakeSeriesBuilder<T>(_map);
            buildAction(builder);
            return Add(builder.Build());
        }

        /// <summary>
        /// Adds a KQL operator to the query.
        /// </summary>
        /// <param name="kqlOperator"></param>
        /// <returns></returns>
        public SelectBuilder<T> Add(IKqlOperator kqlOperator)
        {
            return Add(kqlOperator.ToKql(_parameters));
        }

        /// <summary>
        /// Adds a custom KQL statement to the query.
        /// </summary>
        /// <param name="customKql"></param>
        /// <returns></returns>
        public SelectBuilder<T> Add(string customKql)
        {
            if (_isFirst)
                _isFirst = false;
            else
                _builder.Append(", ");

            _builder.Append(customKql);
            return this;
        }
    }
}
